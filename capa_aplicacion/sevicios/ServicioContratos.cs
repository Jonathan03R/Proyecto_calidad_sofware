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


        public int CrearContrato(ContratoDTO contrato)
        {
            accesoSQLServer.IniciarTransaccion();
            try
            {
                if (contrato == null)
                    throw new ArgumentNullException(nameof(contrato), "el contrato no puede ser nulo.");

                if (!contrato.TrabajadorId.HasValue || contrato.TrabajadorId.Value <= 0)
                    throw new InvalidOperationException("debe seleccionar un trabajador.");

                var activos = contratosRepo.ListarConContratoActivo();

                bool trabajadorTieneContratoActivo = activos
                    .Any(c => c.TrabajadorId == contrato.TrabajadorId);

                if (trabajadorTieneContratoActivo)
                    throw new InvalidOperationException("el trabajador ya tiene un contrato activo.");

                var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    ? contrato.HorasSemanales.Value
                    : 48;

                var entidad = new Contrato
                {
                    Trabajador = new Trabajador { TrabajadorId = contrato.TrabajadorId.Value },
                    Cargo = new Cargo { CargoId = contrato.CargoId ?? throw new InvalidOperationException("debe seleccionar un cargo.") },
                    Area = new Area { AreaId = contrato.AreaId ?? throw new InvalidOperationException("debe seleccionar un área.") },
                    TipoPension = new TipoPension { TipoPensionId = contrato.TipoPensionId ?? throw new InvalidOperationException("debe seleccionar el sistema de pensión.") },
                    TipoSalario = new TipoSalario { TipoSalarioId = contrato.TipoSalarioId ?? throw new InvalidOperationException("debe seleccionar el tipo salarial.") },

                    ContratoFechaInicio = contrato.FechaInicio,
                    ContratoFechaFin = contrato.FechaFin,
                    ContratoSalario = contrato.Salario ?? throw new InvalidOperationException("el salario es obligatorio."),
                    ContratoHorasSemanales = horas,
                    ContratoTarifaHora = contrato.TarifaHora ?? 0
                };

                entidad.ValidarParaCreacion();

                contrato.HorasSemanales = entidad.ContratoHorasSemanales;
                contrato.TarifaHora = entidad.ContratoTarifaHora;

                var id = contratosRepo.CrearContratoEmpleado(contrato);

                accesoSQLServer.TerminarTransaccion();
                return id;
            }
            catch
            {
                accesoSQLServer.CancelarTransaccion();
                throw;
            }
        }

        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
        {
            accesoSQLServer.IniciarTransaccion();

            try
            {
                if (contratoId <= 0)
                    throw new ArgumentException("el id del contrato no es válido.");

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new ArgumentException("el usuario es obligatorio.");

                if (string.IsNullOrWhiteSpace(motivo))
                    throw new ArgumentException("el motivo es obligatorio.");

                var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    ? contrato.HorasSemanales.Value
                    : 48;

                var entidad = new Contrato
                {
                    Trabajador = new Trabajador { TrabajadorId = contrato.TrabajadorId ?? 0 },
                    Cargo = new Cargo { CargoId = contrato.CargoId ?? 0 },
                    Area = new Area { AreaId = contrato.AreaId ?? 0 },
                    TipoPension = new TipoPension { TipoPensionId = contrato.TipoPensionId ?? 0 },
                    TipoSalario = new TipoSalario { TipoSalarioId = contrato.TipoSalarioId ?? 0 },

                    ContratoFechaInicio = contrato.FechaInicio,
                    ContratoFechaFin = contrato.FechaFin,
                    ContratoSalario = contrato.Salario ?? 0,
                    ContratoHorasSemanales = horas,
                    ContratoTarifaHora = contrato.TarifaHora ?? 0
                };

                entidad.ValidarParaCreacion();

                contrato.HorasSemanales = entidad.ContratoHorasSemanales;
                contrato.TarifaHora = entidad.ContratoTarifaHora;

                contratosRepo.ActualizarContrato(contratoId, usuario, motivo, contrato);

                accesoSQLServer.TerminarTransaccion();
            }
            catch
            {
                accesoSQLServer.CancelarTransaccion();
                throw;
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

        public ResumenContratosDto ObtenerResumen()
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                return contratosRepo.ObtenerResumenContratos();
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

        /// se le tiene que pasar por default el periodo actual antes de que carge la pantalla
        public List<ContratoPorPeriodoDTO> ListarContratosPorPeriodo(int periodoId)
        {
            accesoSQLServer.AbrirConexion();
            try
            {
                if (periodoId <= 0)
                    throw new ArgumentException("El ID del periodo no es válido.");

                return contratosRepo.ListarContratosPorPeriodo(periodoId);
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
        }

       
    }
}
