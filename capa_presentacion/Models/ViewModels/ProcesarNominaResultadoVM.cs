using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.ViewModels
{
    public class ProcesarNominaResultadoVM
    {
        public string PeriodoId { get; set; }
        public string PeriodoNombre { get; set; }
        public string TipoPago { get; set; }

        // Colecciones
        public List<EmpleadoResultadoVM> EmpleadosProcesados { get; set; } = new List<EmpleadoResultadoVM>();
        public List<EmpleadoResultadoVM> EmpleadosConError { get; set; } = new List<EmpleadoResultadoVM>();


        // Resumen / Totales
        public int TotalEmpleados => EmpleadosProcesados.Count + EmpleadosConError.Count;
        public decimal TotalBruto => EmpleadosProcesados.Sum(e => e.Bruto);
        public decimal TotalDescuentos => EmpleadosProcesados.Sum(e => e.TotalDescuentos);
        public decimal TotalNeto => EmpleadosProcesados.Sum(e => e.Neto);

        // Estado visible en UI: "Exitoso" | "Con errores"
        public string EstadoFinal { get; set; }
        public string MensajeEstado { get; set; }
    }
}