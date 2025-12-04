using System;

namespace capa_dominio.dto
{
    public class ContratoPorPeriodoDTO
    {
        public int ContratoId { get; set; }
        public int? TrabajadorId { get; set; }
        public string TrabajadorCodigo { get; set; }
        public string PersonaNombre { get; set; }
        public string PersonaApellido { get; set; }
        public decimal? ContratoSalario { get; set; }
        public int? TipoPensionId { get; set; }
        public bool TieneAsignacionFamiliar { get; set; }
        public int? CargoId { get; set; }
        public string CargoNombre { get; set; }
        public int? AreaId { get; set; }
        public string AreaNombre { get; set; }
        public int EstadoContratoId { get; set; }
        public string EstadoContratoNombre { get; set; }
        public int PeriodoId { get; set; }
        public string PeriodoNombre { get; set; }
        public DateTime PeriodoFechaInicio { get; set; }
        public DateTime PeriodoFechaFin { get; set; }
        public bool Procesado { get; set; }
    }
}