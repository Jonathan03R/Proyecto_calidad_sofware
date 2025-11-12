using System;
using System.Collections.Generic;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;

namespace capa_aplicacion.Servicios
{
    public class ServicioContratos
    {
        private readonly AccesoSQLServer accesoSQLServer;
        private readonly ContratosRepositorio contratosRepo;

        public ServicioContratos()
        {
            accesoSQLServer = new AccesoSQLServer();
            contratosRepo = new ContratosRepositorio(accesoSQLServer);
        }

        // ✅ CREAR CONTRATO
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

        // ✅ ACTUALIZAR CONTRATO
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

        // ✅ FINALIZAR CONTRATO
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

        // ✅ CONSULTAR CONTRATOS POR TRABAJADOR
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

        // ✅ CONSULTAR TODOS LOS CONTRATOS
        public List<ContratoDTO> ConsultarTodosLosContratos()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                throw new NotImplementedException("Implementar consulta general de contratos en la capa de persistencia.");
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        //// ✅ LISTAR TRABAJADORES
        public List<TrabajadorDTO> ObtenerTrabajadores()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerTrabajadores();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // ✅ LISTAR ÁREAS
        public List<AreaDTO> ObtenerAreas()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerAreas();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // ✅ LISTAR CARGOS
        public List<CargoDTO> ObtenerCargos()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerCargos();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // ✅ LISTAR ESTADOS DE CONTRATO
        public List<EstadoContratoDTO> ObtenerEstadosContrato()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerEstadosContrato();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        // ✅ LISTAR TIPOS DE PENSIÓN
        public List<TipoPensionDTO> ObtenerTiposPension()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerTiposPension();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }
    }
}
