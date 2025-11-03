using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.ViewModels
{
    public class NominaPeriodoVM
    {
            // Para la vista Index: listado de periodos y parámetros visibles en UI
            public List<PeriodoItemVM> Periodos { get; set; } = new List<PeriodoItemVM>();
            public ParametrosOficialesVM Parametros { get; set; }

            // Para representar un periodo seleccionado (en historial o detalle)
            public string PeriodoId { get; set; }      // Ej: "2025-11"
            public string PeriodoNombre { get; set; }  // Ej: "Noviembre 2025"
            public string TipoPago { get; set; }       // "Mensual" | "Quincenal"
            public bool YaProcesado { get; set; }
            public bool PuedeProcesar { get; set; }
        }

        public class PeriodoItemVM
        {
            public string Id { get; set; }       // "2025-11"
            public string Nombre { get; set; }   // "Noviembre 2025"
            public string TipoPago { get; set; } // "Mensual" | "Quincenal"
            public bool Procesado { get; set; }
        }
    }