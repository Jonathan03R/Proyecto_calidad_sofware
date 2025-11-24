//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace capa_presentacion.Models.ViewModels
//{
//    public class NominaPeriodoVM
//    {
//        // === Catálogo para llenar combos / tablas ===
//        public List<PeriodoItemVM> Periodos { get; set; } = new List<PeriodoItemVM>();

//        // === Parámetros oficiales del sistema (UIT, SMV, AFP, etc) ===
//        public ParametrosOficialesVM Parametros { get; set; }

//        // === Datos del periodo seleccionado ===
//        public string PeriodoId { get; set; }        // Ej: "2025-11"
//        public string PeriodoNombre { get; set; }    // Ej: "Noviembre 2025"
//        public string TipoPago { get; set; }         // "Mensual" | "Quincenal"

//        // === Estado operacional ===
//        public bool YaProcesado { get; set; }
//        public bool PuedeProcesar { get; set; }
//    }

//    public class PeriodoItemVM
//    {
//        public string Id { get; set; }               // "2025-11"
//        public string Nombre { get; set; }           // "Noviembre 2025"
//        public string TipoPago { get; set; }         // "Mensual" | "Quincenal"
//        public bool Procesado { get; set; }
//    }
//}