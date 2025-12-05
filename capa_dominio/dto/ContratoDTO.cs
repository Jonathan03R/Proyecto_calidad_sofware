using System;

namespace capa_dominio.dto
{
    public class ContratoDTO
    {
        public int? ContratoId { get; set; }

        public int? TrabajadorId { get; set; }
        public int? CargoId { get; set; }
        public int? AreaId { get; set; }
        public int? TipoPensionId { get; set; }
        public int? TipoSalarioId { get; set; }
        public int? TipoJornadaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? Salario { get; set; }
        public decimal? TarifaHora { get; set; }
        public int? HorasSemanales { get; set; }
        public int? ModoPagoId { get; set; }
        public string ModoPagoNombre { get; set; }
        public string DocumentoUrl { get; set; }
        public string DescripcionFunciones { get; set; }
        public string Observaciones { get; set; }
        public string EmpleadoNombre { get; set; }
        public string Documento { get; set; }
        public string CargoNombre { get; set; }
        public string EstadoContratoNombre { get; set; }
    }
}
