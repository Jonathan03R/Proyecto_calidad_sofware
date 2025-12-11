using capa_presentacion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.Mock
{
    /// <summary>
    /// Datos falsos para renderizar la UI sin BD ni capa aplicación.
    /// </summary>
    public static class DemoData
    {
        public static List<PeriodoItemVM> ObtenerPeriodos()
        {
            return new List<PeriodoItemVM>
            {
                new PeriodoItemVM { Id = "2025-09", Nombre = "Septiembre 2025", TipoPago = "Mensual",  Procesado = true  },
                new PeriodoItemVM  { Id = "2025-10", Nombre = "Octubre 2025",    TipoPago = "Mensual",  Procesado = true  },
                new PeriodoItemVM  { Id = "2025-11", Nombre = "Noviembre 2025",  TipoPago = "Mensual",  Procesado = false },
                new PeriodoItemVM { Id = "2025-11-Q2", Nombre = "Quincena 2 - Nov 2025", TipoPago = "Quincenal", Procesado = false }
            };
        }

        public static ParametrosOficialesVM ObtenerParametrosOficiales()
        {
            return new ParametrosOficialesVM
            {
                RMV = 1025m,
                UIT = 5150m,
                AsignacionFamiliarPct = 0.10m,
                EsSaludPct = 0.09m,
                ONPPct = 0.13m,
                AFPFondoPct = 0.10m,
                AFPComisionPct = 0.015m,
                AFPSeguroPct = 0.017m,
                TablaIR = DemoBuilders.TramosIRDemo()
            };
        }

        public static ProcesarNominaResultadoVM SimularProcesamiento(string periodoId)
        {
            var periodos = ObtenerPeriodos();
            var seleccionado = periodos.FirstOrDefault(p => p.Id == periodoId) ?? periodos.Last();

            // Empleados OK
            var ok = new List<EmpleadoResultadoVM>
            {
                DemoBuilders.EmpleadoOk(101, "Ana Rojas", 1600, esOnp: true),
                DemoBuilders.EmpleadoOk(102, "Carlos Díaz", 1800, esOnp: false),
                DemoBuilders.EmpleadoOk(103, "María Pérez", 1500, esOnp: false),
            };

            // Empleados con error (para _TablaErrores)
            var conError = new List<EmpleadoResultadoVM>
            {
                DemoBuilders.EmpleadoConError(201, "Luis Castro", "Falta contrato activo"),
                DemoBuilders.EmpleadoConError(202, "Julia Vega", "Parámetros oficiales incompletos"),
            };

            return new ProcesarNominaResultadoVM
            {
                PeriodoId = seleccionado.Id,
                PeriodoNombre = seleccionado.Nombre,
                TipoPago = seleccionado.TipoPago,
                EmpleadosProcesados = ok,
                EmpleadosConError = conError,
                EstadoFinal = conError.Any() ? "Con errores" : "Exitoso",
                MensajeEstado = conError.Any()
                    ? "Se procesó parcialmente. Revise la pestaña de Errores."
                    : "Procesamiento completado correctamente."
            };
        }

        public static List<NominaPeriodoVM> ObtenerHistorialNominas()
        {
            // Historial simple para la vista de lista
            return new List<NominaPeriodoVM>
            {
                new NominaPeriodoVM { PeriodoId = "2025-09", PeriodoNombre = "Septiembre 2025", TipoPago = "Mensual", YaProcesado = true,  PuedeProcesar = false },
                new NominaPeriodoVM { PeriodoId = "2025-10", PeriodoNombre = "Octubre 2025",    TipoPago = "Mensual", YaProcesado = true,  PuedeProcesar = false },
                new NominaPeriodoVM { PeriodoId = "2025-11", PeriodoNombre = "Noviembre 2025",  TipoPago = "Mensual", YaProcesado = false, PuedeProcesar = true  },
            };
        }
    }
}