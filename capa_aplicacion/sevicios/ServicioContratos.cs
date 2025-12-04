using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;

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
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

                // ============================================================
                // VALIDACIÓN: MÍNIMO 3 MESES
                // ============================================================
                if (contrato.FechaInicio != DateTime.MinValue &&
                    contrato.FechaFin.HasValue && contrato.FechaFin != DateTime.MinValue)
                {
                    var inicio = contrato.FechaInicio;
                    var fin = contrato.FechaFin.Value;

                    int meses = ((fin.Year - inicio.Year) * 12) + (fin.Month - inicio.Month);

                    if (meses < 3)
                        throw new Exception("El tiempo mínimo de contrato debe ser de 3 meses.");
                }
                // ============================================================


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
                TipoJornada = new TipoJornada
                {
                    TipoJornadaId = contrato.TipoJornadaId ?? 0
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

            return contratosRepo.CrearContratoEmpleado(contrato);
        }

        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
        {
            if (contratoId <= 0)
                throw new ArgumentException("El ID del contrato no es válido.");

            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Debe indicar el usuario que realiza la actualización.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe indicar el motivo de la actualización.");

                // ============================================================
                // VALIDACIÓN: MÍNIMO 3 MESES
                // ============================================================
                if (contrato.FechaInicio != DateTime.MinValue &&
                    contrato.FechaFin.HasValue && contrato.FechaFin != DateTime.MinValue)
                {
                    var inicio = contrato.FechaInicio;
                    var fin = contrato.FechaFin.Value;

                    int meses = ((fin.Year - inicio.Year) * 12) + (fin.Month - inicio.Month);

                    if (meses < 3)
                        throw new Exception("El tiempo mínimo de contrato debe ser de 3 meses.");
                }
                // ============================================================


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
                TipoJornada = new TipoJornada
                {
                    TipoJornadaId = contrato.TipoJornadaId ?? 0
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


        public (int contratoActualizado, int cambioRegistrado) FinalizarContrato(int contratoId, string observaciones = null)
        {
            if (contratoId <= 0)
                throw new ArgumentException("El ID del contrato no es válido.");

            return contratosRepo.FinalizarContrato(contratoId, observaciones);
        }


        public List<Contrato> ConsultarContratosPorTrabajador(int trabajadorId)
        {
            if (trabajadorId <= 0)
                throw new ArgumentException("El ID del trabajador no es válido.");

            return contratosRepo.ObtenerContratosPorTrabajador(trabajadorId);
        }

        public List<ContratoDTO> ListarContratosActivos()
        {
            return contratosRepo.ListarConContratoActivo();
        }


        public List<ContratoDTO> ObtenerContratosFiltrados(int? estadoContratoId, string buscar)
        {
            return contratosRepo.ObtenerContratosFiltrados(estadoContratoId, buscar);
        }


        public List<ContratoDTO> ListarSinContratoActivo()
        {
            return contratosRepo.ListarSinContratoActivo();
        }

        public ResumenContratosDto ObtenerResumen()
        {
            return contratosRepo.ObtenerResumenContratos();
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
