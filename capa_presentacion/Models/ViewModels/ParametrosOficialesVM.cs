using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.ViewModels
{
    public class ParametrosOficialesVM
    { 
        // Valores visibles en el modal de Parámetros Oficiales
        public decimal RMV { get; set; }
        public decimal UIT { get; set; }
        public decimal AsignacionFamiliarPct { get; set; }
        public decimal EsSaludPct { get; set; }

        // Aportes/Descuentos previsionales
        public decimal ONPPct { get; set; }
        public decimal AFPFondoPct { get; set; }
        public decimal AFPComisionPct { get; set; }
        public decimal AFPSeguroPct { get; set; }

        // Tabla de Impuesto a la Renta (tramos)
        public List<ImpuestoRentaTramoVM> TablaIR { get; set; } = new List<ImpuestoRentaTramoVM>();
    }

    public class ImpuestoRentaTramoVM
    {
        public decimal DesdeUIT { get; set; }    // Límite inferior en UIT
        public decimal HastaUIT { get; set; }    // Límite superior en UIT (0 = sin tope)
        public decimal TasaPct { get; set; }     // % del tramo
        public decimal DeduccionFija { get; set; } // Si aplica (0 por defecto)
    }
}