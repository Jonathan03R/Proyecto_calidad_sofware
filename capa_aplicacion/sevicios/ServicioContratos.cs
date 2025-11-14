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


        public int CrearContrato(ContratoDTO contrato)

        {
            accesoSQLServer.IniciarTransaccion();
            try
            {
                if (contrato == null)
                    throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

                var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    ? contrato.HorasSemanales.Value
                    : 48;

                var entidad = new Contrato
                {
                    Trabajador = new Trabajador
                    {
                        TrabajadorId = contrato.TrabajadorId ?? 0
                    },
                    Cargo = new Cargo
                    {
                        CargoId = contrato.CargoId ?? 0
                    },
                    Area = new Area
                    {
                        AreaId = contrato.AreaId ?? 0
                    },
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = contrato.TipoPensionId ?? 0
                    },
                    TipoSalario = new TipoSalario
                    {
                        TipoSalarioId = contrato.TipoSalarioId ?? 0
                    },
                    ContratoFechaInicio = contrato.FechaInicio,
                    ContratoFechaFin = contrato.FechaFin,
                    ContratoSalario = contrato.Salario ?? 0,
                    ContratoHorasSemanales = horas,
                    ContratoTarifaHora = contrato.TarifaHora ?? 0  
                };

                entidad.ValidarParaCreacion();

                contrato.HorasSemanales = entidad.ContratoHorasSemanales;
                contrato.TarifaHora = entidad.ContratoTarifaHora;

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

                var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    ? contrato.HorasSemanales.Value
                    : 48;

                var entidad = new Contrato
                {
                    Trabajador = new Trabajador
                    {
                        TrabajadorId = contrato.TrabajadorId ?? 0
                    },
                    Cargo = new Cargo
                    {
                        CargoId = contrato.CargoId ?? 0
                    },
                    Area = new Area
                    {
                        AreaId = contrato.AreaId ?? 0
                    },
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = contrato.TipoPensionId ?? 0
                    },
                    TipoSalario = new TipoSalario
                    {
                        TipoSalarioId = contrato.TipoSalarioId ?? 0
                    },

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

        public ResumenContratosDTO ObtenerResumen()
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