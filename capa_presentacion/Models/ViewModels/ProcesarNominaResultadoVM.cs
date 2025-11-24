using System.Collections.Generic;

namespace capa_presentacion.Models.ViewModels
{
    public class ProcesarNominaResultadoVM
    {
        // ==============================
        // DATOS DEL PERIODO
        // ==============================
        public string PeriodoId { get; set; }
        public string PeriodoNombre { get; set; }
        public string TipoPago { get; set; }

        // ==============================
        // ESTADO DEL PROCESAMIENTO
        // ==============================
        public string EstadoFinal { get; set; }  // Ej: "Exitoso", "Con errores", "Procesando"
        public string MensajeEstado { get; set; }

        // ==============================
        // LISTAS DE RESULTADOS
        // (Son opcionales; las llenarás con tu servicio)
        // ==============================
        public List<EmpleadoResultadoVM> EmpleadosProcesados { get; set; }
            = new List<EmpleadoResultadoVM>();

        public List<EmpleadoResultadoVM> EmpleadosConError { get; set; }
            = new List<EmpleadoResultadoVM>();

        // ==============================
        // TOTALES
        // ==============================
        public int TotalEmpleados =>
            (EmpleadosProcesados?.Count ?? 0) + (EmpleadosConError?.Count ?? 0);

        public decimal TotalBruto { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal TotalNeto { get; set; }
    }
}
