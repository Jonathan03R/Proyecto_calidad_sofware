using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.ViewModels
{
    public class EmpleadoResultadoVM
    {
        // Identificación
        public int Codigo { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }

        // Ingresos
        public decimal SueldoBasico { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal Bonos { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal Bruto => SueldoBasico + HorasExtras + Bonos + OtrosIngresos;

        // Descuentos
        public bool EsONP { get; set; }        // true = ONP, false = AFP (solo para UI)
        public decimal DescuentoONP { get; set; }
        public decimal DescuentoAFP_Fondo { get; set; }
        public decimal DescuentoAFP_Comision { get; set; }
        public decimal DescuentoAFP_Seguro { get; set; }
        public decimal ImpuestoRentaMes { get; set; }
        public decimal Adelantos { get; set; }
        public decimal Tardanzas { get; set; }
        public decimal Faltas { get; set; }
        public decimal OtrosDescuentos { get; set; }
        public decimal TotalDescuentos =>
            DescuentoONP + DescuentoAFP_Fondo + DescuentoAFP_Comision + DescuentoAFP_Seguro +
            ImpuestoRentaMes + Adelantos + Tardanzas + Faltas + OtrosDescuentos;

        // Aportes del empleador (solo informativos en UI)
        public decimal EsSalud { get; set; }

        // Neto
        public decimal Neto => Bruto - TotalDescuentos;

        // Estado de fila para tablas de Errores
        public bool TieneErrores { get; set; }
        public string MensajeError { get; set; }
    }
}