using System;

namespace capa_dominio
{
    public class Contrato
    {
        private int contratoId;
        private Trabajador trabajador;
        private Cargo cargo;
        private Area area;
        private TipoPension tipoPension;
        private TipoSalario tipoSalario;
        private TipoJornada tipoJornada;
        private int estadoiId;
        private DateTime contratoFechaInicio;
        private DateTime? contratoFechaFin;
        private int? contratoHorasSemanales;
        private decimal contratoTarifaHora;
        private decimal contratoSalario;
        private string contratoModoPago;
        private string contratoDocumentoUrl;
        private string contratoDescripcionFunciones;
        private string contratoObservaciones;
        private DateTime contratoFechaCreacion;

        public int ContratoId { get => contratoId; set => contratoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public Cargo Cargo { get => cargo; set => cargo = value; }
        public Area Area { get => area; set => area = value; }
        public TipoPension TipoPension { get => tipoPension; set => tipoPension = value; }
        public TipoSalario TipoSalario { get => tipoSalario; set => tipoSalario = value; }
        public TipoJornada TipoJornada { get => tipoJornada; set => tipoJornada = value; }
        public int EstadoiId { get => estadoiId; set => estadoiId = value; }
        public DateTime ContratoFechaInicio { get => contratoFechaInicio; set => contratoFechaInicio = value; }
        public DateTime? ContratoFechaFin { get => contratoFechaFin; set => contratoFechaFin = value; }
        public int? ContratoHorasSemanales { get => contratoHorasSemanales; set => contratoHorasSemanales = value; }

        public decimal ContratoTarifaHora
        {
            get => contratoTarifaHora;
            set
            {
                if (value < 0)
                    throw new ArgumentException("La tarifa por hora no puede ser negativa.");
                contratoTarifaHora = value;
            }
        }

        public decimal ContratoSalario
        {
            get => contratoSalario;
            set
            {
                if (value < 0)
                    throw new ArgumentException("El salario no puede ser negativo.");
                contratoSalario = value;
            }
        }

        public string ContratoModoPago { get => contratoModoPago; set => contratoModoPago = value; }
        public string ContratoDocumentoUrl { get => contratoDocumentoUrl; set => contratoDocumentoUrl = value; }
        public string ContratoDescripcionFunciones { get => contratoDescripcionFunciones; set => contratoDescripcionFunciones = value; }
        public string ContratoObservaciones { get => contratoObservaciones; set => contratoObservaciones = value; }
        public DateTime ContratoFechaCreacion { get => contratoFechaCreacion; set => contratoFechaCreacion = value; }

        public bool EsActivo()
        {
            return EstadoiId == 1;
        }


        // Asumimos 6 días laborales por semana
        public decimal ObtenerJornadaDiaria()
        {
            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
                throw new InvalidOperationException("El contrato no tiene configuradas las horas semanales.");

            return Math.Round(ContratoHorasSemanales.Value / 6m, 2);
        }

        public decimal ObtenerSueldoPorDia()
        {
            if (ContratoSalario <= 0)
                throw new InvalidOperationException("El salario del contrato no está definido o es inválido.");

            return Math.Round(ContratoSalario / 30m, 2);
        }

        public void CalcularTarifaHora()
        {
            if (ContratoSalario <= 0)
                throw new InvalidOperationException("El salario es inválido.");

            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
                throw new InvalidOperationException("Las horas semanales no están configuradas.");

            var jornadaDiaria = ContratoHorasSemanales.Value / 6m;

            if (jornadaDiaria <= 0)
                throw new InvalidOperationException("La jornada diaria es inválida.");

            ContratoTarifaHora = Math.Round(
                ContratoSalario / (30m * jornadaDiaria),
                2
            );
        }

        public void ValidarParaCreacion()
        {
            if (Trabajador == null || Trabajador.TrabajadorId <= 0)
                throw new InvalidOperationException("Debe seleccionar un trabajador.");

            if (Cargo == null || Cargo.CargoId <= 0)
                throw new InvalidOperationException("Debe seleccionar un cargo.");

            if (Area == null || Area.AreaId <= 0)
                throw new InvalidOperationException("Debe seleccionar un área.");

            if (TipoPension == null || TipoPension.TipoPensionId <= 0)
                throw new InvalidOperationException("Debe seleccionar el sistema de pensiones.");

            if (TipoSalario == null || TipoSalario.TipoSalarioId <= 0)
                throw new InvalidOperationException("Debe seleccionar el tipo de salario.");

            if (ContratoFechaInicio == DateTime.MinValue)
                throw new InvalidOperationException("Debe especificar una fecha de inicio válida.");

            if (ContratoFechaFin.HasValue && ContratoFechaFin.Value < ContratoFechaInicio)
                throw new InvalidOperationException("La fecha de fin no puede ser anterior a la fecha de inicio.");

            if (ContratoSalario <= 0)
                throw new InvalidOperationException("El salario debe ser mayor a 0.");

            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
                throw new InvalidOperationException("Las horas semanales deben ser mayores a 0.");

            CalcularTarifaHora();
        }


    }
}
