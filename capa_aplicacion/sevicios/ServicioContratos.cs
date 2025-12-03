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

        public ResumenContratosDTO ObtenerResumen()
        {
            return contratosRepo.ObtenerResumenContratos();
        }

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
