using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio
{
    public class HoraTrabajada
    {
        private DateTime fecha;
        private decimal horasNormales;
        private decimal horasExtras;
        private decimal horasDescanso;
        private decimal totalDiaTrabajado;
        private Contrato contrato;
        private List<TipoHoraExtra> tiposHorasExtras;

        public DateTime Fecha { get => fecha; set => fecha = value; }
        public decimal HorasNormales { get => horasNormales; set => horasNormales = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal HorasDescanso { get => horasDescanso; set => horasDescanso = value; }
        public decimal TotalDiaTrabajado { get => totalDiaTrabajado; set => totalDiaTrabajado = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public List<TipoHoraExtra> TiposHorasExtras { get => tiposHorasExtras; set => tiposHorasExtras = value; }

        public decimal CalcularPagoHorasExtras()
        {
            if (Contrato == null)
                throw new InvalidOperationException("Contrato nulo.");

            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
                throw new InvalidOperationException("Tipos de horas extras no cargadas.");

            decimal tarifaHora = Contrato.ContratoTarifaHora;
            decimal pagoExtras = 0m;

            decimal recPrimeras2 = ObtenerMultiplicador("PRIMERAS2");     // 0.25
            decimal recAdicionales = ObtenerMultiplicador("ADICIONALES"); // 0.35
            decimal recSabado = ObtenerMultiplicador("SABADO");           // 0.50
            decimal recDomingo = ObtenerMultiplicador("DOMINGO");         // 1.00

            var dia = Fecha.DayOfWeek;

            // DOMINGO -> TODO lo extra se paga 100% adicional
            if (dia == DayOfWeek.Sunday)
            {
                decimal tarifaConRecargo = tarifaHora * (1 + recDomingo);
                pagoExtras = HorasExtras * tarifaConRecargo;
                return Math.Round(pagoExtras, 2);
            }

            // SÁBADO -> todas las extras con recargo SABADO
            if (dia == DayOfWeek.Saturday)
            {
                decimal tarifaConRecargo = tarifaHora * (1 + recSabado);
                pagoExtras = HorasExtras * tarifaConRecargo;
                return Math.Round(pagoExtras, 2);
            }

            // LUNES A VIERNES -> primeras 2 y adicionales
            if (HorasExtras > 0)
            {
                decimal primerasDos = Math.Min(HorasExtras, 2);
                decimal adicionales = Math.Max(HorasExtras - 2, 0);

                decimal pagoP2 = primerasDos * tarifaHora * (1 + recPrimeras2);
                decimal pagoAdi = adicionales * tarifaHora * (1 + recAdicionales);

                pagoExtras = pagoP2 + pagoAdi;
            }

            return Math.Round(pagoExtras, 2);
        }




        private decimal ObtenerMultiplicador(string codigo)
        {
            var tipo = TiposHorasExtras.FirstOrDefault(t =>
                t.TiposHorasExtrasCodigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)
                && t.TiposHorasExtrasEstado == 'A');

            if (tipo == null)
                throw new InvalidOperationException($"Falta el tipo de hora extra '{codigo}' (o está inactivo).");

            return tipo.TiposHorasExtrasMultiplicador;
        }



        public decimal CalcularDescuentoTardanza()
        {
            if (contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular descuentos.");

            decimal jornada_diaria = contrato.ObtenerJornadaDiaria();
            decimal sueldo_por_dia = contrato.ObtenerSueldoPorDia();

            if (jornada_diaria <= 0)
                return 0;

            // si no trabajó nada, no contar como tardanza
            if (HorasNormales == 0)
                return 0;

            if (HorasNormales >= jornada_diaria)
                return 0;

            decimal horas_tardanza = jornada_diaria - HorasNormales;
            decimal descuento_por_hora = sueldo_por_dia / jornada_diaria;

            return horas_tardanza * descuento_por_hora;
        }

    }
}
