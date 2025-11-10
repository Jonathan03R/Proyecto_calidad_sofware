using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ReporteNominaDTO
    {
        // ----------------------------------------------------
        //        1. Datos del Trabajador (RN-01)
        // ----------------------------------------------------
        public string CodigoTrabajador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoDeIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string SistemaPension { get; set; }
        public string TipoTrabajador { get; set; }
        public string FechaInicioContrato { get; set; }
        public string FechaFinContrato { get; set; } 

        // ----------------------------------------------------
        //        2. Jornada Laboral (RN-08)
        // ----------------------------------------------------
        public string TipoDeJornadaPactada { get; set; }
        public decimal? HorasSemanalesPactadas { get; set; } // ✅ CAMBIADO A NULLABLE
        public decimal? HorasTrabajadasEstimadas { get; set; } // ✅ CAMBIADO A NULLABLE
        public decimal? HorasExtrasReales { get; set; } // Resultado del SUM subquery

        // ----------------------------------------------------
        //        3. Ingresos (RN-02)
        // ----------------------------------------------------
        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal MontoHorasExtras { get; set; }
        public decimal MontoBonos { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal TotalHaberesBruto { get; set; }
        public decimal TotalHaberes { get; set; }

        // ----------------------------------------------------
        //        4. Descuentos, Aportes y Totales
        // ----------------------------------------------------
        public decimal AporteSistemaPension { get; set; }
        public decimal DescuentoONP { get; set; }
        public decimal DescuentoAFP { get; set; }
        public decimal RetencionImpuestoRenta { get; set; }

        // Aportes del Empleador (RN-05)
        public decimal AporteEsSalud { get; set; }
        public decimal BaseImponibleEsSalud { get; set; }

        // Otros Descuentos
        public decimal DescuentoFaltas { get; set; }
        public decimal DescuentoAdelantos { get; set; }
        public decimal OtrosDescuentos { get; set; }

        // Totales Finales
        public decimal TotalDescuentos { get; set; }
        public decimal NetoPagar { get; set; }

        // ----------------------------------------------------
        //        5. Datos de la Nómina (Contexto)
        // ----------------------------------------------------
        public string PeriodoNomina { get; set; }
    }
}
