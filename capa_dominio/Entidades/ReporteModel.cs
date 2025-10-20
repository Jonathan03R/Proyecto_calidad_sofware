using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.Entidades
{
    public class ReporteModel
    {
        // RN-01: Datos Completos del Trabajador
        public string CodigoTrabajador { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string TipoTrabajador { get; set; }
        public string SistemaPension { get; set; }
        public DateTime FechaInicioContrato { get; set; }

        // RN-08: Jornada Laboral Cumplida
        public string TipoDeJornadaPactada { get; set; }
        public decimal HorasTrabajadasEstimadas { get; set; }
        public decimal HorasExtrasReales { get; set; }

        // RN-02: Ingresos Percibidos (Remuneración Total)
        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal MontoHorasExtras { get; set; }
        public decimal MontoBonos { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal TotalHaberesBruto { get; set; }

        // RN-04: Descuentos Legales y Otros Descuentos
        public decimal AporteSistemaPension { get; set; }
        public decimal RetencionImpuestoRenta { get; set; }
        public decimal DescuentoTardanzas { get; set; }
        public decimal DescuentoAdelantos { get; set; }
        public decimal TotalDescuentos { get; set; }

        // RN-05: Aportes Obligatorios del Empleador (EsSalud)
        public decimal BaseImponibleEsSalud { get; set; }
        public decimal AporteEsSalud { get; set; }
        public decimal NetoPagar { get; set; }
        public string PeriodoNomina { get; set; }
        public DateTime FechaGeneracion { get; set; }


    }
}
