using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ReporteNominaDTO
    {

        public string CodigoTrabajador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoDeIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string SistemaPension { get; set; }
        public string TipoTrabajador { get; set; }
        public string FechaInicioContrato { get; set; }
        public string FechaFinContrato { get; set; } 
        public decimal? HorasSemanalesPactadas { get; set; } 
        public decimal? HorasExtrasReales { get; set; } 
        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal MontoHorasExtras { get; set; }
        public decimal MontoBonos { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal TotalHaberesBruto { get; set; }
        public decimal TotalHaberes { get; set; }
        public decimal AporteSistemaPension { get; set; }
        public decimal DescuentoComision { get; set; }
        public decimal RetencionImpuestoRenta { get; set; }
        public decimal AporteEsSalud { get; set; }
        public decimal BaseImponibleEsSalud { get; set; }
        public decimal DescuentoTardanzas { get; set; }
        public decimal DescuentoFaltas { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal NetoPagar { get; set; }
    }
}
