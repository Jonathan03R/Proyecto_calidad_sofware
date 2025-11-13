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
        /// Procesa la nómina completa para un período específico, incluyendo la validación de existencia previa,
        /// obtención de trabajadores con contratos activos, cálculo de detalles individuales y finalización de totales.
        /// Este método es invocado por la capa de presentación para ejecutar el procesamiento automático de nóminas.
        /// </summary>
        /// <param name="periodoId">Identificador único del período para el cual se procesará la nómina. No puede ser nulo.</param>
        /// <param name="tramos">Lista de tramos de impuesto a la renta vigentes, utilizados para calcular el impuesto correspondiente.</param>
        /// <param name="parametroEssalud">Parámetro que contiene la configuración para el cálculo del aporte a Essalud.</param>
        /// <param name="valorUIT">Valor actual de la Unidad Impositiva Tributaria (UIT), expresado en soles, utilizado en cálculos fiscales.</param>
        /// <exception cref="ArgumentException">Se lanza si <paramref name="periodoId"/> es nulo.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe una nómina exitosa o en proceso para el período especificado.</exception>
        /// <remarks>
        /// El método opera dentro de una transacción de base de datos para asegurar la integridad de los datos.
        /// En caso de error, la transacción se revierte y el estado de la nómina se actualiza a "Con Errores".
        /// </remarks>
        
        public void ProcesarNominaPorPeriodo(int? periodoId, List<ImpuestoRentaTramo> tramos, Parametro parametroEssalud, decimal valorUIT)
        {
            if (periodoId == null)
                throw new ArgumentException("Selecciona un periodo.");

            var nomina = new Nomina
            {
                Periodo = new Periodo { PeriodoId = periodoId.Value },
                Detalles = new List<DetalleNomina>()
            };

            try
            {
                _conexion.IniciarTransaccion();

                ValidarExistenciaNomina(periodoId.Value);

                var (fechaInicio, fechaFin) = RangoFechasPeriodo(periodoId.Value);

                var trabajadores = ObtenerTrabajadoresConContratoActivo();

                var tiposHorasExtras = _tiposHorasExtras.ObtenerTiposHorasExtrasActivos();

                var nominaId = CrearCabeceraNomina(periodoId.Value);
                nomina.NominaId = nominaId;
                nomina.NominaEstado = "Procesando";

                var huboErrores = ProcesarDetallesNomina(nomina, trabajadores, tramos, parametroEssalud, valorUIT , fechaInicio, fechaFin);

                FinalizarNomina(nomina, huboErrores);

                _conexion.TerminarTransaccion();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR NOMINA: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("STACKTRACE: " + ex.StackTrace);
                _conexion.CancelarTransaccion();
                if (nomina.NominaId > 0)
                    _nominas.ActualizarEstado(nomina.NominaId, "Con Errores");

                throw;
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }


        private (DateTime FechaInicio, DateTime FechaFin) RangoFechasPeriodo(int periodoId)
        {
            var periodo = _periodos.ObtenerPeriodoPorId(periodoId);

            if (periodo == null)
                throw new Exception($"El periodo {periodoId} no existe.");

            return (periodo.PeriodoFechaInicio, periodo.PeriodoFechaFin);
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

                if (contratoActivo != null)
                {
                    trabajador.Contrato = contratoActivo;
                    resultado.Add(trabajador);
                }
            }

            return resultado;
        }
        private int CrearCabeceraNomina(int periodoId)
        {
            return _nominas.IniciarProcesoPorPeriodo(periodoId, "Nómina generada automáticamente");
        }

        private bool ProcesarDetallesNomina(
            Nomina nomina,
            List<Trabajador> trabajadores,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            DateTime fechaInicio,
            DateTime fechaFin)
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
                        fechaInicio,
                        fechaFin
                    );

                    var detalle = new DetalleNomina
                    {
                        Nomina = nomina,
                        Contrato = contrato,
                        HorasTrabajadas = horasTrabajadas,
                        TiposHorasExtras = tiposHorasExtras,
                        BonosRegulares = 0,
                        OtrosIngresos = 0
                    };

                    // 1) sueldo según asistencia (aquí se prorratea)
                    detalle.CalcularSueldoSegunAsistencia(fechaInicio, fechaFin);

                    // 2) asignación familiar
                    detalle.CalculoAsignacionFamiliar(trabajador.TieneDerechoAsignacionFamiliar());

                    // 3) horas extras
                    detalle.CalcularPagoTotalHorasExtras();

                    // 4) remuneración bruta
                    detalle.CalcularRemuneracionBruta();

                    // 5) pensiones
                    detalle.CalcularSistemaPensiones();

                    // 6) Essalud
                    detalle.CalcularAporteEssalud(parametroEssalud);

                    // 7) renta de quinta
                    detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

                    // 8) totales
                    detalle.CalcularTotales();

                    var dto = new DetalleNominaDTO(
                        nomina.NominaId,
                        trabajador.TrabajadorId,
                        detalle.RemuneracionBruta,
                        detalle.SueldoBasico,
                        detalle.AsignacionFamiliar,
                        detalle.HorasExtras,
                        detalle.BonosRegulares,
                        detalle.OtrosIngresos,
                        detalle.SistemasPensionAplicado ?? "No definido",
                        detalle.AporteEssalud,
                        detalle.AporteONP,
                        detalle.DescuentoAFP,
                        0, // RemuneraciónAcumuladaAnual
                        0, // BaseImponibleAnual
                        0, // ImpuestoRentaAnual
                        detalle.ImpuestoRentaMensual,
                        valorUIT,
                        0,                         // Deducción7Uit
                        0,                         // DescuentoTardanzas
                        detalle.DescuentoFaltas,   // DescuentoFaltas
                        detalle.DescuentoAdelantos,// DescuentoAdelantos
                        0,                         // OtrosDescuentos
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
                    System.Diagnostics.Trace.WriteLine(
                        $"Error procesando trabajador {trabajador.TrabajadorId}: {ex.Message}"
                    );
                    throw;
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
    }
}
