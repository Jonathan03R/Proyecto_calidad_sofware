using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_aplicacion.servicios
{
    public class NominasServicios
    {
        private readonly AccesoSQLServer _conexion;
        private readonly NominasRepositorio _nominas;
        private readonly DetallesNominaRepositorio _detalleNomina;
        private readonly TrabajadoresRepositorio _trabajadores;
        private readonly HijosRepositorio _hijos;
        private readonly ContratoRepositorio _contratos;
        private readonly HorasTrabajadasRepositorio _horasTrabajadas;
        private readonly TiposHorasExtrasRepositorio _tiposHorasExtras;
        private readonly PeriodosRepositorio _periodos;

        public NominasServicios()
        {
            _conexion = new AccesoSQLServer();
            _nominas = new NominasRepositorio(_conexion);
            _detalleNomina = new DetallesNominaRepositorio(_conexion);
            _trabajadores = new TrabajadoresRepositorio(_conexion);
            _hijos = new HijosRepositorio(_conexion);
            _contratos = new ContratoRepositorio(_conexion);
            _horasTrabajadas = new HorasTrabajadasRepositorio(_conexion);
            _tiposHorasExtras = new TiposHorasExtrasRepositorio(_conexion);
            _periodos = new PeriodosRepositorio(_conexion);
        }

        /// <summary>
        /// Procesa la nómina para un periodo determinado
        /// </summary>
        public void ProcesarNominaPorPeriodo(
            int? periodoId,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            decimal montoAsignacionFamiliar)
        {
            if (periodoId == null)
                throw new ArgumentException("Selecciona un periodo.");

            _conexion.IniciarTransaccion();

            var periodo = _periodos.ObtenerPeriodoPorId(periodoId.Value);
            if (periodo == null)
                throw new InvalidOperationException($"El periodo {periodoId.Value} no existe.");

            if (periodo.EsProcesado())
                throw new InvalidOperationException("El periodo ya está procesado.");

            var nomina = new Nomina
            {
                Periodo = periodo,
                Detalles = new List<DetalleNomina>()
            };

            try
            {
                ValidarExistenciaNomina(periodo.PeriodoId);
                var trabajadores = ObtenerTrabajadoresConContratoActivo();

                var nominaId = CrearCabeceraNomina(periodo.PeriodoId);
                nomina.NominaId = nominaId;
                nomina.NominaEstado = "Procesando";

                var huboErrores = ProcesarDetallesNomina(
                    nomina,
                    trabajadores,
                    tramos,
                    parametroEssalud,
                    valorUIT,
                    montoAsignacionFamiliar);

                FinalizarNomina(nomina, huboErrores);
                _periodos.ProcesarPeriodo(periodo.PeriodoId);

                _conexion.TerminarTransaccion();
            }
            catch
            {
                _conexion.CancelarTransaccion();
                if (nomina.NominaId > 0)
                    _nominas.ActualizarEstado(nomina.NominaId, "Con Errores");
                throw;
            }
        }

        private void ValidarExistenciaNomina(int periodoId)
        {
            var nominas = _nominas.ObtenerNominasEnPeriodo(periodoId);
            if (!nominas.Any()) return;

            var nomina = nominas.First();
            if (nomina.EsExitosa())
                throw new InvalidOperationException($"La nómina ya fue procesada exitosamente para el periodo {periodoId}.");
            if (nomina.EstaProcesando())
                throw new InvalidOperationException($"Ya existe una nómina en proceso para el periodo {periodoId}.");
        }

        private List<Trabajador> ObtenerTrabajadoresConContratoActivo()
        {
            var trabajadores = _trabajadores.ObtenerEmpleados(1, int.MaxValue);
            var resultado = new List<Trabajador>();

            foreach (var trabajador in trabajadores)
            {
                var contratos = _contratos.ObtenerContratosPorTrabajador(trabajador.TrabajadorId);
                var contratoActivo = contratos.FirstOrDefault(c => c.EsActivo());
                if (contratoActivo == null) continue;

                trabajador.Contrato = contratoActivo;
                resultado.Add(trabajador);
            }

            return resultado;
        }

        private int CrearCabeceraNomina(int periodoId)
        {
            return _nominas.IniciarProcesoPorPeriodo(periodoId, "Nómina generada automáticamente");
        }

        /// <summary>
        /// ✅ ORDEN CORRECTO DE CÁLCULOS:
        /// 1. Asignación familiar
        /// 2. Horas extras
        /// 3. Descuento tardanzas
        /// 4. Descuento faltas (excluye sábado y domingo)
        /// 5. Bonos regulares (después de tardanzas y faltas)
        /// 6. Remuneración bruta
        /// 7. Sistema de pensiones
        /// 8. Essalud (sobre base imponible: sueldo + asig. familiar)
        /// 9. Renta quinta
        /// 10. Totales
        /// </summary>
        private bool ProcesarDetallesNomina(
            Nomina nomina,
            List<Trabajador> trabajadores,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            decimal montoAsignacionFamiliar)
        {
            bool algunError = false;
            var tiposHorasExtras = _tiposHorasExtras.ObtenerTiposHorasExtrasActivos();

            foreach (var trabajador in trabajadores)
            {
                try
                {
                    trabajador.Hijos = _hijos.ObtenerHijosPorTrabajador(trabajador.TrabajadorId);

                    var contrato = trabajador.Contrato;
                    if (contrato == null)
                        continue;

                    var horasTrabajadas = _horasTrabajadas.ObtenerHorasTrabajadas(
                        contrato.ContratoId,
                        nomina.Periodo.PeriodoFechaInicio,
                        nomina.Periodo.PeriodoFechaFin
                    );

                    var detalle = new DetalleNomina
                    {
                        Nomina = nomina,
                        Contrato = contrato,
                        SueldoBasico = contrato.ContratoSalario,
                        HorasTrabajadas = horasTrabajadas,
                        TiposHorasExtras = tiposHorasExtras,
                        BonosRegulares = 0,
                        OtrosIngresos = 0
                    };

                    // ✅ ORDEN CORRECTO:

                    // 1. Asignación familiar
                    detalle.CalculoAsignacionFamiliar(
                        trabajador.TieneDerechoAsignacionFamiliar(),
                        montoAsignacionFamiliar);

                    // 2. Horas extras
                    detalle.CalcularPagoTotalHorasExtras();

                    // 3. Descuento tardanzas
                    detalle.CalcularDescuentoTardanzas();

                    // 4. Descuento faltas (excluye sábado y domingo)
                    detalle.CalcularDescuentoFaltas();

                    // 5. Bonos regulares (después de tardanzas y faltas)
                    detalle.CalculoBonosRegulares();

                    // 6. Remuneración bruta
                    detalle.CalcularRemuneracionBruta();

                    // 7. Sistema de pensiones
                    detalle.CalcularSistemaPensiones();

                    // 8. Essalud (sobre base imponible)
                    detalle.CalcularAporteEssalud(parametroEssalud);

                    // 9. Renta quinta
                    detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

                    // 10. Totales
                    detalle.CalcularTotales();

                    // ✅ CREAR DTO CON NUEVO PARÁMETRO
                    var dto = new DetalleNominaDTO(
                        nomina.NominaId,
                        contrato.ContratoId,
                        detalle.RemuneracionBruta,
                        detalle.SueldoBasico,
                        detalle.AsignacionFamiliar,
                        detalle.HorasExtras,
                        detalle.BonosRegulares,
                        detalle.OtrosIngresos,
                        detalle.SistemasPensionAplicado ?? "No definido",
                        detalle.AporteEssalud,
                        detalle.BaseImponibleEsSalud, // ✅ NUEVO
                        detalle.AporteONP,
                        detalle.DescuentoAFP,
                        0,      // remuneración acumulada anual
                        0,      // base imponible anual
                        0,      // impuesto renta anual
                        detalle.ImpuestoRentaMensual,
                        valorUIT,
                        0,      // deducción 7 UIT
                        detalle.DescuentoTardanzas,
                        detalle.DescuentoFaltas,
                        detalle.DescuentoAdelantos,
                        0,      // otros descuentos
                        detalle.TotalIngresos,
                        detalle.TotalDescuentos,
                        detalle.NetoPagar,
                        false,
                        string.Empty
                    );

                    _detalleNomina.InsertarDetalleNomina(dto);
                    nomina.Detalles.Add(detalle);
                }
                catch (Exception ex)
                {
                    algunError = true;

                    var dtoError = new DetalleNominaDTO(
                        nomina.NominaId,
                        trabajador.Contrato.ContratoId,
                        ex.Message
                    );

                    _detalleNomina.InsertarDetalleNomina(dtoError);
                }
            }

            return algunError;
        }

        private void FinalizarNomina(Nomina nomina, bool huboErrores)
        {
            nomina.CalcularTotales();

            var estado = huboErrores ? "Incompleto" : "Exitoso";

            _nominas.ActualizarTotales(
                nomina.NominaId,
                nomina.NominaTotalEmpleados,
                nomina.NominaTotalBruto,
                nomina.NominaTotalDescuentos,
                nomina.NominaTotalNeto,
                estado
            );
        }

        public List<NominasProcesadasDTO> ListarDetallesNominasProcesadas(
            int? trabajadorId = null,
            int? nominaId = null,
            int? periodoId = null,
            string estadoNomina = null)
        {
            List<NominasProcesadasDTO> listaDetalles;
            try
            {
                _conexion.AbrirConexion();
                listaDetalles = _detalleNomina.ListarDetallesNominasProcesadas(
                    trabajadorId,
                    nominaId,
                    periodoId,
                    estadoNomina);
                _conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listaDetalles;
        }

        public List<ContratoPorPeriodoDTO> ListarContratosPorPeriodo(int periodoId)
        {
            _conexion.AbrirConexion();
            try
            {
                if (periodoId <= 0)
                    throw new ArgumentException("El ID del periodo no es válido.");

                return _contratos.ListarContratosPorPeriodo(periodoId);
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }

        public List<ResumenNominaDTO> ListarResumenNominas()
        {
            _conexion.AbrirConexion();

            try
            {
                return _nominas.ListarResumenNominas();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }
    }
}