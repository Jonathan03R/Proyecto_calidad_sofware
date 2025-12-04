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
        private readonly SistemasPensionesRepositorio _sistemasPensionesRepositorio;

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
            _sistemasPensionesRepositorio = new SistemasPensionesRepositorio(_conexion);
        }

        // ============================================================
        // PROCESAMIENTO POR TRABAJADOR (unitario)
        // - este método realiza todo el cálculo y guarda el detalle
        // - la transacción es por trabajador (no por todo el periodo)
        // ============================================================
        public ResultadoEmpleadoDTO ProcesarEmpleadoIndividual(
            int nominaId,
            int trabajadorId,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            Periodo periodo
        )
        {
            _conexion.IniciarTransaccion();

            Contrato contrato = null;

            try
            {
                contrato = _contratos.ObtenerContratosPorTrabajador(trabajadorId)
                                     .FirstOrDefault(c => c.EsActivo());
                
                if (contrato == null)
                {
                    _conexion.CancelarTransaccion();
                    return new ResultadoEmpleadoDTO(trabajadorId, false, "sin contrato activo");
                }
                var sistemasPensiones = _sistemasPensionesRepositorio.ObtenerSistemasPensiones();

                contrato.TipoPension = sistemasPensiones
                    .FirstOrDefault(p => p.TipoPensionId == contrato.TipoPension.TipoPensionId);

                var hijos = _hijos.ObtenerHijosPorTrabajador(trabajadorId);

                var horasTrabajadas = _horasTrabajadas.ObtenerHorasTrabajadas(
                    contrato.ContratoId,
                    periodo.PeriodoFechaInicio,
                    periodo.PeriodoFechaFin
                );

                var tiposHorasExtras = _tiposHorasExtras.ObtenerTiposHorasExtrasActivos();

                var detalle = new DetalleNomina
                {
                    Nomina = new Nomina { NominaId = nominaId, Periodo = periodo },
                    Contrato = contrato,
                    SueldoBasico = contrato.ContratoSalario,
                    HorasTrabajadas = horasTrabajadas,
                    TiposHorasExtras = tiposHorasExtras,
                    BonosRegulares = 0,
                    OtrosIngresos = 0
                };

                detalle.CalculoAsignacionFamiliar(hijos != null && hijos.Count > 0);
                detalle.CalcularPagoTotalHorasExtras();
                detalle.CalcularDescuentoTardanzas();
                detalle.CalcularDescuentoFaltas();
                detalle.CalcularRemuneracionBruta();
                detalle.CalcularSistemaPensiones();
                detalle.CalcularAporteEssalud(parametroEssalud);
                detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);
                detalle.CalcularTotales();

                var dto = new DetalleNominaDTO(
                    nominaId,
                    contrato.ContratoId,
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
                    0,
                    0,
                    0,
                    detalle.ImpuestoRentaMensual,
                    valorUIT,
                    0,
                    detalle.DescuentoTardanzas,
                    detalle.DescuentoFaltas,
                    detalle.DescuentoAdelantos,
                    0,
                    detalle.TotalIngresos,
                    detalle.TotalDescuentos,
                    detalle.NetoPagar,
                    false,
                    string.Empty
                );

                _detalleNomina.InsertarDetalleNomina(dto);

                _conexion.TerminarTransaccion();

                return new ResultadoEmpleadoDTO(trabajadorId, true, "ok");
            }
            catch (Exception ex)
            {
                _conexion.CancelarTransaccion();

                var contratoId = contrato != null ? contrato.ContratoId : 0;

                var dtoError = new DetalleNominaDTO(
                    nominaId,
                    contratoId,
                    ex.Message
                );

                // guardamos el detalle de error para auditoría
                _detalleNomina.InsertarDetalleNomina(dtoError);

                return new ResultadoEmpleadoDTO(trabajadorId, false, ex.Message);
            }
        }

        // ============================================================
        // INICIAR PROCESO PARA EL PERIODO (crea cabecera de nómina
        // y devuelve la lista de trabajadores pendientes)
        // ============================================================


        public (int nominaId, List<int> trabajadoresPendientes) IniciarProcesoYObtenerPendientes(int periodoId)
        {
            if (periodoId <= 0)
                throw new ArgumentException("Periodo inválido.");

            _conexion.IniciarTransaccion();

            try
            {
                var periodo = _periodos.ObtenerPeriodoPorId(periodoId);
                if (periodo == null)
                    throw new InvalidOperationException($"Periodo {periodoId} no existe.");

                if (periodo.EsProcesado())
                    throw new InvalidOperationException("El periodo ya está procesado.");

                // 1️⃣ Validar estado de nómina previa
                int nominaId = ObtenerNominaValidaParaPeriodo(periodoId);

                // 2️⃣ Crear nueva solo si no hay una válida
                if (nominaId == 0)
                {
                    nominaId = _nominas.IniciarProcesoPorPeriodo(
                        periodoId,
                        "Nómina generada automáticamente"
                    );
                }

                // 3️⃣ Obtener pendientes
                var pendientes = ObtenerTrabajadoresPendientes(nominaId, periodoId);

                if (pendientes == null || pendientes.Count == 0)
                {
                    _conexion.CancelarTransaccion();
                    throw new InvalidOperationException("No hay trabajadores pendientes para este periodo.");
                }

                _conexion.TerminarTransaccion();
                return (nominaId, pendientes);
            }
            catch
            {
                _conexion.CancelarTransaccion();
                throw;
            }
        }

        public List<int> ObtenerTrabajadoresPendientes(int nominaId, int periodoId)
        {
            if (nominaId <= 0)
                throw new ArgumentException("Nomina inválida.");

            if (periodoId <= 0)
                throw new ArgumentException("Periodo inválido.");

            var contratos = _contratos.ListarContratosPorPeriodo(periodoId);
            var pendientes = new List<int>();

            foreach (var c in contratos)
            {
                if (!c.TrabajadorId.HasValue)
                    continue;

                bool yaProcesado = _detalleNomina.ExisteDetalleParaContrato(nominaId, c.ContratoId);

                if (!yaProcesado)
                    pendientes.Add(c.TrabajadorId.Value);
            }

            return pendientes;
        }

        // ============================================================
        // PROCESAR UN TRABAJADOR DENTRO DE UNA NOMINA (wrapper)
        // - el frontend llamará a este endpoint por cada trabajador
        // - requiere periodoId para obtener fechas y contexto
        // ============================================================
        public ResultadoEmpleadoDTO ProcesarTrabajadorEnNomina(
            int nominaId,
            int trabajadorId,
            int periodoId,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT
        )
        {
            _conexion.AbrirConexion();

            try
            {
                // 1. cargar periodo
                var periodo = _periodos.ObtenerPeriodoPorId(periodoId);
                if (periodo == null)
                    return new ResultadoEmpleadoDTO(trabajadorId, false, "Periodo no encontrado.");

                // 2. obtener contrato activo
                var contratos = _contratos.ObtenerContratosPorTrabajador(trabajadorId);
                var contratoActivo = contratos.FirstOrDefault(c => c.EsActivo());
                if (contratoActivo == null)
                    return new ResultadoEmpleadoDTO(trabajadorId, false, "sin contrato activo");

                // 3. validar si ya fue procesado
                bool yaProcesado = _detalleNomina.ExisteDetalleParaContrato(nominaId, contratoActivo.ContratoId);
                if (yaProcesado)
                    return new ResultadoEmpleadoDTO(trabajadorId, false, "ya procesado");

                // 4. delegar al método real (este maneja su propia transacción)
                return ProcesarEmpleadoIndividual(
                    nominaId,
                    trabajadorId,
                    tramos,
                    parametroEssalud,
                    valorUIT,
                    periodo
                );
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }


        // ============================================================
        // CERRAR NOMINA Y ACTUALIZAR ESTADO DEL PERIODO
        // - llamar después de procesar todos los trabajadores pendientes
        // - si huboErrores == true -> marca periodo con estado de incidencias
        // - si huboErrores == false -> marca periodo como procesado
        // ============================================================
        public void CerrarNominaYActualizarPeriodo(int nominaId, int periodoId, bool? huboErrores, bool cancelado = false)
        {
            if (nominaId <= 0)
                throw new ArgumentException("Nomina inválida.");

            if (periodoId <= 0)
                throw new ArgumentException("Periodo inválido.");

            _conexion.IniciarTransaccion();

            try
            {
                string estadoFinal = "Exitoso";

                if (cancelado)
                {
                    estadoFinal = "Cancelado";

                    _nominas.ActualizarEstado(nominaId, estadoFinal);
                    _periodos.ActualizarEstadoPeriodo(periodoId, 5);

                    _conexion.TerminarTransaccion();
                    return;
                }

                bool erroresFinal = huboErrores.HasValue
                    ? huboErrores.Value
                    : CalcularPendientes(nominaId, periodoId);

                estadoFinal = erroresFinal ? "Con Errores" : "Exitoso";

                // ================================
                // CÁLCULO DE TOTALES
                // ================================
                var detalles = _detalleNomina.ListarDetallesPorNomina(nominaId);

                var nomina = new Nomina
                {
                    NominaId = nominaId,
                    Detalles = detalles
                };

                nomina.CalcularTotales();

                // ================================
                // ACTUALIZAR TOTALES *CORRECTAMENTE*
                // ================================
                _nominas.ActualizarTotales(
                    nominaId,
                    nomina.NominaTotalEmpleados,
                    nomina.NominaTotalBruto,
                    nomina.NominaTotalDescuentos,
                    nomina.NominaTotalNeto,
                    estadoFinal    
                );

                // ================================
                // ACTUALIZAR ESTADO DEL PERIODO
                // ================================
                if (!erroresFinal)
                    _periodos.ProcesarPeriodo(periodoId);
                else
                    _periodos.ActualizarEstadoPeriodo(periodoId, 5);

                _conexion.TerminarTransaccion();
            }
            catch (Exception ex)
            {
                _conexion.CancelarTransaccion();
                throw new Exception("Error al cerrar nómina y actualizar periodo.", ex);
            }
        }


        private bool CalcularPendientes(int nominaId, int periodoId)
        {
            var pendientes = ObtenerTrabajadoresPendientes(nominaId, periodoId);
            return pendientes != null && pendientes.Count > 0;
        }

        // ============================================================
        // HELPERS y MÉTODOS DE APOYO
        // ============================================================


        private int ObtenerNominaValidaParaPeriodo(int periodoId)
        {
            var nominas = _nominas.ObtenerNominasEnPeriodo(periodoId);

            if (!nominas.Any())
                return 0;

            var nomina = nominas.First();

            if (nomina.EstaProcesando())
                throw new InvalidOperationException(
                    $"Ya existe una nómina EN PROCESO para el periodo {periodoId}. Debes finalizarla primero."
                );

            return nomina.NominaId;
        }

        public List<Trabajador> ObtenerTrabajadoresConContratoActivo()
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

        //private int CrearCabeceraNomina(int periodoId)
        //{
        //    return _nominas.IniciarProcesoPorPeriodo(periodoId, "Nómina generada automáticamente");
        //}

        // reutiliza tu proc/consulta ya existente que devuelve contratos por periodo
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

        // filtra contratos que realmente queremos procesar
        public List<ContratoPorPeriodoDTO> ObtenerContratosParaPeriodo(int periodoId)
        {
            var contratos = ListarContratosPorPeriodo(periodoId);

            var contratosFiltrados = contratos
                .Where(c =>
                    c.TrabajadorId.HasValue &&
                    !c.Procesado &&         // flag 'procesado' debe venir del proc
                    c.EstadoContratoId == 1 // 1 = activo (ajusta si tu dominio usa otro id)
                )
                .ToList();

            return contratosFiltrados;
        }

        // ids de trabajadores a procesar
        public List<int> ObtenerTrabajadorIdsParaPeriodo(int periodoId)
        {
            return ObtenerContratosParaPeriodo(periodoId)
                .Where(c => c.TrabajadorId.HasValue)
                .Select(c => c.TrabajadorId.Value)
                .Distinct()
                .ToList();
        }

        // listado simple de resumen (usa tu repo existente)
        public List<ResumenNominaDto> ListarResumenNominas()
        {
            _conexion.AbrirConexion();
            try
            {
                return _nominas.ListarResumenNominas();
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }

        // detalles procesados (usa tu repo existente)
        public List<NominasProcesadasDTO> ListarDetallesNominasProcesadas(
            int? trabajadorId = null,
            int? nominaId = null,
            int? periodoId = null,
            string estadoNomina = null)
        {
            _conexion.AbrirConexion();
            try
            {
                return _detalleNomina.ListarDetallesNominasProcesadas(trabajadorId, nominaId, periodoId, estadoNomina);
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }

        public ResumenKpisNominaDto ObtenerResumenKpisNomina()
        {
            // Simplemente delega al repositorio
            return _nominas.ObtenerResumenKpisNomina();
        }
    }
}
