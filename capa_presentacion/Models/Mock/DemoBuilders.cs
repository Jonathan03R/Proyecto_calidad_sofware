using capa_presentacion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace capa_presentacion.Models.Mock
{
    /// <summary>
    /// Pequeñas fábricas para crear filas y estructuras de prueba (solo UI).
    /// </summary>
    public static class DemoBuilders
    {
        public static EmpleadoResultadoVM EmpleadoOk(
            int codigo, string nombre, decimal basico, bool esOnp = true)
        {
            return new EmpleadoResultadoVM
            {
                Codigo = codigo,
                Documento = "DNI" + codigo.ToString("00000000"),
                Nombre = nombre,
                SueldoBasico = basico,
                HorasExtras = 120,
                Bonos = 150,
                OtrosIngresos = 0,
                EsONP = esOnp,
                DescuentoONP = esOnp ? basico * 0.13m : 0m,
                DescuentoAFP_Fondo = esOnp ? 0m : basico * 0.10m,
                DescuentoAFP_Comision = esOnp ? 0m : basico * 0.015m,
                DescuentoAFP_Seguro = esOnp ? 0m : basico * 0.017m,
                ImpuestoRentaMes = 80,
                Adelantos = 0,
                Tardanzas = 0,
                Faltas = 0,
                OtrosDescuentos = 0,
                EsSalud = basico * 0.09m,
                TieneErrores = false,
                MensajeError = null
            };
        }

        public static EmpleadoResultadoVM EmpleadoConError(
            int codigo, string nombre, string mensajeError)
        {
            var e = EmpleadoOk(codigo, nombre, 1300);
            e.TieneErrores = true;
            e.MensajeError = mensajeError;
            return e;
        }

        public static List<ImpuestoRentaTramoVM> TramosIRDemo()
        {
            return new List<ImpuestoRentaTramoVM>
            {
                new ImpuestoRentaTramoVM { DesdeUIT = 0, HastaUIT = 5, TasaPct = 0, DeduccionFija = 0 },
                new ImpuestoRentaTramoVM { DesdeUIT = 5, HastaUIT = 20, TasaPct = 8, DeduccionFija = 0 },
                new ImpuestoRentaTramoVM { DesdeUIT = 20, HastaUIT = 35, TasaPct = 14, DeduccionFija = 0 },
                new ImpuestoRentaTramoVM { DesdeUIT = 35, HastaUIT = 45, TasaPct = 17, DeduccionFija = 0 },
                new ImpuestoRentaTramoVM { DesdeUIT = 45, HastaUIT = 0, TasaPct = 20, DeduccionFija = 0 }, // 0 = sin tope
            };
        }
    }
}