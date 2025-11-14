using capa_dominio;
using capa_dominio.dto;
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

        // CREAR CONTRATO
        public int CrearContrato(ContratoDTO contrato)
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                if (contrato == null)
                    throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

                if (contrato.FechaInicio == DateTime.MinValue)
                    throw new ArgumentException("Debe especificar una fecha de inicio válida.");

                if (contrato.FechaFin.HasValue && contrato.FechaFin < contrato.FechaInicio)
                    throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");

                if ((contrato.Salario ?? 0) <= 0 && (contrato.TarifaHora ?? 0) <= 0)
                    throw new ArgumentException("Debe especificar un salario o una tarifa por hora válida.");

                return contratosRepo.CrearContratoEmpleado(contrato);
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // ACTUALIZAR CONTRATO
        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
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

        public List<ContratoDTO> ListarContratosActivos()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ListarConContratoActivo();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        public List<ContratoDTO> ListarSinContratoActivo()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ListarSinContratoActivo();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
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

                var trabajadores = trabajadorService.ObtenerEmpleados();
                datos.Trabajador = trabajadores.FirstOrDefault(t => t.TrabajadorId == trabajadorId);

                if (datos.Trabajador == null)
                    throw new Exception("No se encontró el trabajador con ID " + trabajadorId);

                datos.Areas = areaService.ObtenerAreas();
                datos.Cargos = cargoService.ObtenerCargos();
                datos.Pensiones = pensionService.ObtenerSistemasPensiones();

                datos.TiposJornada = new List<TipoJornada>();
                datos.TiposSalario = new List<TipoSalario>();

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
