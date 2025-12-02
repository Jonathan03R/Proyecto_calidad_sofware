using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{

    public class PaginacionResultadoDTO<T>
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }

    public class NominasProcesadasPaginadoDTO
    {
        public int NominaId { get; set; }
        public int PeriodoId { get; set; }
        public string PeriodoNombre { get; set; }

        public DateTime PeriodoFechaInicio { get; set; }
        public DateTime PeriodoFechaFin { get; set; }

        public DateTime NominaFecha { get; set; }
        public DateTime NominaFechaProcesamiento { get; set; }

        public string NominaEstado { get; set; }
        public int NominaTotalEmpleados { get; set; }
        public decimal NominaTotalBruto { get; set; }
        public decimal NominaTotalDescuentos { get; set; }
        public decimal NominaTotalNeto { get; set; }

        public string NominaObservaciones { get; set; }
    }


    public class NominasProcesadasDTO
    {
        public int DetalleNominaId { get; set; }
        public int NominaId { get; set; }
        public int TrabajadorId { get; set; }
        public int ContratoId { get; set; }
        public int PeriodoId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NominaEstado { get; set; }
        public string EstadoContratoNombre { get; set; }
        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal BonosRegulares { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal RemuneracionBruta { get; set; }
        public decimal TotalIngresos { get; set; }
        public string SistemaPensionAplicado { get; set; }
        public decimal AporteEssalud { get; set; }
        public decimal AporteOnp { get; set; }
        public decimal DescuentoAfp { get; set; }
        public decimal RemuneracionAcumuladaAnual { get; set; }
        public decimal BaseImponibleAnual { get; set; }
        public decimal ImpuestoRentaAnual { get; set; }
        public decimal ImpuestoRentaMensual { get; set; }
        public decimal UitValor { get; set; }
        public decimal Deduccion7Uit { get; set; }
        public decimal DescuentoTardanzas { get; set; }
        public decimal DescuentoFaltas { get; set; }
        public decimal DescuentoAdelantos { get; set; }
        public decimal OtrosDescuentos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal NetoPagar { get; set; }
    }
}
