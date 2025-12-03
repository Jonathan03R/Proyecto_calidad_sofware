using System;

namespace capa_dominio.dto
{
    public class ContratoDTO
    {
        public int? ContratoId { get; set; }

        private int? trabajadorId;
        private int? cargoId;
        private int? areaId;
        private int? tipoPensionId;
        private int? tipoSalarioId;


        private DateTime fechaInicio;
        private DateTime? fechaFin;

        private decimal? salario;
        private decimal? tarifaHora;

        private int modoPagoId;
        private string modoPagoNombre;
        private string documentoUrl;
        private string descripcionFunciones;
        private string observaciones;

        public int? TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public int? CargoId { get => cargoId; set => cargoId = value; }
        public int? AreaId { get => areaId; set => areaId = value; }
        public int? TipoPensionId { get => tipoPensionId; set => tipoPensionId = value; }
        public int? TipoSalarioId { get => tipoSalarioId; set => tipoSalarioId = value; }
        public DateTime FechaInicio { get => fechaInicio; set => fechaInicio = value; }
        public DateTime? FechaFin { get => fechaFin; set => fechaFin = value; }
        public decimal? Salario { get => salario; set => salario = value; }
        public decimal? TarifaHora { get => tarifaHora; set => tarifaHora = value; }
        public int? HorasSemanales { get; set; }
        public int ModoPagoId { get => modoPagoId; set => modoPagoId = value; }
        public string ModoPagoNombre { get => modoPagoNombre; set => modoPagoNombre = value; }
        public string DocumentoUrl { get => documentoUrl; set => documentoUrl = value; }
        public string DescripcionFunciones { get => descripcionFunciones; set => descripcionFunciones = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }
        public string EmpleadoNombre { get; set; }
        public string Documento { get; set; }
        public string CargoNombre { get; set; }
        public string EstadoContratoNombre { get; set; }
    }
}
