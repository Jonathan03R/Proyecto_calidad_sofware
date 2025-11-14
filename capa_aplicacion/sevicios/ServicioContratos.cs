using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_aplicacion.Servicios
{
    public class ServicioContratos
    {
        private readonly AccesoSQLServer accesoSQLServer;
        private readonly ContratoRepositorio contratosRepo;

        public ServicioContratos()
        {
            accesoSQLServer = new AccesoSQLServer();
            contratosRepo = new ContratoRepositorio(accesoSQLServer);
        }

        // CREAR CONTRATO (ahora usa Contrato, no ContratoDTO)
        public int CrearContrato(Contrato contrato)
        {
            accesoSQLServer.IniciarTransaccion();
            try
            {
                if (contrato == null)
                    throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

                if (contrato.ContratoFechaInicio == DateTime.MinValue)
                    throw new ArgumentException("Debe especificar una fecha de inicio válida.");

                if (contrato.ContratoFechaFin.HasValue && contrato.ContratoFechaFin < contrato.ContratoFechaInicio)
                    throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");

                if (contrato.ContratoSalario <= 0 && contrato.ContratoTarifaHora <= 0)
                    throw new ArgumentException("Debe especificar un salario o una tarifa por hora válida.");

                int resultado = contratosRepo.CrearContratoEmpleado(contrato);
                accesoSQLServer.TerminarTransaccion();
                return resultado;
            }
            catch (Exception ex)
            {
                accesoSQLServer.CancelarTransaccion();
                throw new Exception($"Error al crear contrato: {ex.Message}", ex);
            }
        }

        // ✅ ACTUALIZAR CONTRATO (ahora usa Contrato, no ContratoDTO)
        public void ActualizarContrato(int contratoId, string usuario, string motivo, Contrato contrato)
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                if (contratoId <= 0)
                    throw new ArgumentException("El ID del contrato no es válido.");

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new ArgumentException("Debe indicar el usuario que realiza la actualización.");

                if (string.IsNullOrWhiteSpace(motivo))
                    throw new ArgumentException("Debe indicar el motivo de la actualización.");

                if (contrato == null)
                    throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

                contratosRepo.ActualizarContrato(contratoId, usuario, motivo, contrato);
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // FINALIZAR CONTRATO
        public (int contratoActualizado, int cambioRegistrado) FinalizarContrato(int contratoId, string observaciones = null)
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                if (contratoId <= 0)
                    throw new ArgumentException("El ID del contrato no es válido.");

                return contratosRepo.FinalizarContrato(contratoId, observaciones);
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // CONSULTAR CONTRATOS POR TRABAJADOR
        public List<Contrato> ConsultarContratosPorTrabajador(int trabajadorId)
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                if (trabajadorId <= 0)
                    throw new ArgumentException("El ID del trabajador no es válido.");

                return contratosRepo.ObtenerContratosPorTrabajador(trabajadorId);
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // LISTAR ACTIVOS (devuelve objetos dinámicos para la vista)
        public List<dynamic> ListarContratosActivos()
        {
            var contratos = contratosRepo.ListarConContratoActivo();

            // Convertir a objetos anónimos para la vista
            return contratos.Select(c => new
            {
                ContratoId = c.ContratoId,
                EmpleadoNombre = c.Trabajador != null ? $"{c.Trabajador.Nombres} {c.Trabajador.Apellidos}" : "",
                Documento = c.Trabajador?.Identificacion ?? "",
                CargoNombre = c.Cargo?.CargoNombre ?? "",
                EstadoContratoNombre = ObtenerNombreEstado(c.EstadoiId),
                FechaInicio = c.ContratoFechaInicio,
                FechaFin = c.ContratoFechaFin,
                CargoId = c.Cargo?.CargoId,
                TipoSalarioId = c.TipoSalario?.TipoSalarioId,
                Salario = c.ContratoSalario,
                ModoPago = c.ContratoModoPago,
                Observaciones = c.ContratoObservaciones
            } as dynamic).ToList();
        }

        // LISTAR SIN CONTRATO
        public List<dynamic> ListarSinContratoActivo()
        {
            var trabajadores = contratosRepo.ListarSinContratoActivo();

            return trabajadores.Select(t => new
            {
                TrabajadorId = t.TrabajadorId,
                EmpleadoNombre = $"{t.Nombres} {t.Apellidos}",
                Documento = t.Identificacion,
                EstadoContratoNombre = "Sin Contrato"
            } as dynamic).ToList();
        }

        // Método auxiliar para nombres de estado
        private string ObtenerNombreEstado(int estadoId)
        {
            switch (estadoId)
            {
                case 1: return "Activo";
                case 2: return "Finalizado";
                case 3: return "Suspendido";
                case 4: return "Inactivo";
                default: return "Desconocido";
            }
        }

        // OBTENER DATOS COMPLETOS PARA NUEVO CONTRATO
        public DatosNuevoContrato ObtenerDatosParaNuevoContrato(int trabajadorId)
        {
            try
            {
                var datos = new DatosNuevoContrato();

                var trabajadorService = new capa_aplicacion.sevicios.TrabajadorService();
                var areaService = new capa_aplicacion.sevicios.AreaService();
                var cargoService = new capa_aplicacion.sevicios.CargoService();
                var pensionService = new capa_aplicacion.sevicios.PensionService();
                var tipoSalarioService = new capa_aplicacion.sevicios.Tipos_salarios.TipoSalarioServicio();
                var tipoJornadaService = new capa_aplicacion.sevicios.TipoJornadaService();

                var trabajadores = trabajadorService.ObtenerEmpleados();
                datos.Trabajador = trabajadores.FirstOrDefault(t => t.TrabajadorId == trabajadorId);

                if (datos.Trabajador == null)
                    throw new Exception("No se encontró el trabajador con ID " + trabajadorId);

                datos.Areas = areaService.ObtenerAreas();
                datos.Cargos = cargoService.ObtenerCargos();
                datos.Pensiones = pensionService.ObtenerSistemasPensiones();
                datos.TiposSalario = tipoSalarioService.ObtenerTiposSalarios();
                datos.TiposJornada = tipoJornadaService.ObtenerTiposJornadas()
                    .Select(j => new capa_dominio.TipoJornada
                    {
                        TipoJornadaId = j.TipoJornadaId,
                        TipoJornadaNombre = j.TipoJornadaNombre,
                        TipoJornadaDescripcion = j.TipoJornadaDescripcion,
                        TipoJornadaEstado = j.TipoJornadaEstado,
                        TipoJornadaFechaCreacion = j.TipoJornadaFechaCreacion
                    })
                    .ToList();

                return datos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo datos para nuevo contrato: " + ex.Message, ex);
            }
        }

        // CLASE CONTENEDORA
        public class DatosNuevoContrato
        {
            public Trabajador Trabajador { get; set; }
            public List<Area> Areas { get; set; }
            public List<Cargo> Cargos { get; set; }
            public List<TipoPension> Pensiones { get; set; }
            public List<TipoSalario> TiposSalario { get; set; }
            public List<TipoJornada> TiposJornada { get; set; }
        }
    }
}