using System;

namespace capa_dominio
{
    public class Contrato
    {

        public int ContratoId { get; set; }
        public Trabajador Trabajador { get; set; }
        public Cargo Cargo { get; set; }
        public Area Area { get; set; }
        public TipoPension TipoPension { get; set; }
        public TipoSalario TipoSalario { get; set; }
        public TipoJornada TipoJornada { get; set; }
        public int EstadoiId { get; set; }
        public DateTime ContratoFechaInicio { get; set; }
        public DateTime? ContratoFechaFin { get; set; }
        public int? ContratoHorasSemanales { get; set; }
        public string ContratoModoPago { get; set; }
        public string ContratoDocumentoUrl { get; set; }
        public string ContratoDescripcionFunciones { get; set; }
        public string ContratoObservaciones { get; set; }
        public DateTime ContratoFechaCreacion { get; set; }

        // ====== Propiedades con validación ======

        private decimal contratoTarifaHora;
        public decimal ContratoTarifaHora
        {
            get => contratoTarifaHora;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("La tarifa por hora no puede ser negativa.");
                }

                contratoTarifaHora = value;
            }
        }

        private decimal contratoSalario;
        public decimal ContratoSalario
        {
            get => contratoSalario;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El salario no puede ser negativo.");
                }

                contratoSalario = value;
            }
        }

        // ====== Reglas de negocio ======

        public bool EsActivo() => EstadoiId == 1;

        public decimal ObtenerJornadaDiaria()
        {
            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
            {
                throw new InvalidOperationException("El contrato no tiene configuradas las horas semanales.");
            }

            return Math.Round(ContratoHorasSemanales.Value / 6m, 2);
        }

        public decimal ObtenerSueldoPorDia()
        {
            if (ContratoSalario <= 0)
            {
                throw new InvalidOperationException("El salario del contrato no está definido o es inválido.");
            }

            return Math.Round(ContratoSalario / 30m, 2);
        }

        public void CalcularTarifaHora()
        {
            if (ContratoSalario <= 0)
            {
                throw new InvalidOperationException("El salario es inválido.");
            }

            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
            {
                throw new InvalidOperationException("Las horas semanales no están configuradas.");
            }

            var jornadaDiaria = ContratoHorasSemanales.Value / 6m;

            if (jornadaDiaria <= 0)
            {
                throw new InvalidOperationException("La jornada diaria es inválida.");
            }

            ContratoTarifaHora = Math.Round(
                ContratoSalario / (30m * jornadaDiaria),
                2
            );
        }

        // ====== Validaciones agrupadas para bajar Cognitive Complexity ======

        public void ValidarParaCreacion()
        {
            ValidarEntidadesRequeridas();
            ValidarFechas();
            ValidarSalarioYHoras();
            CalcularTarifaHora();
        }

        private void ValidarEntidadesRequeridas()
        {
            if (Trabajador?.TrabajadorId <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar un trabajador.");
            }

            if (Cargo?.CargoId <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar un cargo.");
            }

            if (Area?.AreaId <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar un área.");
            }

            if (TipoPension?.TipoPensionId <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar el sistema de pensiones.");
            }

            //if (TipoSalario?.TipoSalarioId <= 0)
            //{
            //    throw new InvalidOperationException("Debe seleccionar el tipo de salario.");
            //}
        }

        private void ValidarFechas()
        {
            if (ContratoFechaInicio == DateTime.MinValue)
            {
                throw new InvalidOperationException("Debe especificar una fecha de inicio válida.");
            }

            if (ContratoFechaFin.HasValue && ContratoFechaFin.Value < ContratoFechaInicio)
            {
                throw new InvalidOperationException("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }
        }

        private void ValidarSalarioYHoras()
        {
            if (ContratoSalario <= 0)
            {
                throw new InvalidOperationException("El salario debe ser mayor a 0.");
            }

            if (!ContratoHorasSemanales.HasValue || ContratoHorasSemanales.Value <= 0)
            {
                throw new InvalidOperationException("Las horas semanales deben ser mayores a 0.");
            }
        }
    }
}
