using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio
{
    public class HoraTrabajada
    {
        public DateTime Fecha { get ; set ; }
        public decimal HorasNormales { get ; set ; }
        public decimal HorasExtras { get ; set ; }
        public decimal HorasDescanso { get ; set ; }
        public decimal TotalDiaTrabajado { get ; set ; }
        public Contrato Contrato { get ; set ; }
        public List<TipoHoraExtra> TiposHorasExtras { get ; set ; }

        public decimal CalcularPagoHorasExtras()
        {
            if (Contrato == null)
                throw new InvalidOperationException("Contrato nulo.");

            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
                throw new InvalidOperationException("Tipos de horas extras no cargadas.");

            decimal tarifaHora = Contrato.ContratoTarifaHora;
            decimal pagoExtras = 0m;

            decimal recPrimeras2 = ObtenerMultiplicador("PRIMERAS2");
            decimal recAdicionales = ObtenerMultiplicador("ADICIONALES");
            decimal recSabado = ObtenerMultiplicador("SABADO");
            decimal recDomingo = ObtenerMultiplicador("DOMINGO");

            var dia = Fecha.DayOfWeek;

            if (dia == DayOfWeek.Sunday)
            {
                decimal tarifaConRecargo = tarifaHora * (1 + recDomingo);
                pagoExtras = HorasExtras * tarifaConRecargo;
                return pagoExtras;
            }

            if (HorasExtras > 0)
            {
                decimal primerasDos = Math.Min(HorasExtras, 2);
                decimal adicionales = Math.Max(HorasExtras - 2, 0);

                decimal pagoP2 = primerasDos * tarifaHora * (1 + recPrimeras2);
                decimal pagoAdi = adicionales * tarifaHora * (1 + recAdicionales);

                pagoExtras = pagoP2 + pagoAdi;
            }

            return pagoExtras;
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
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular descuentos.");

            decimal jornada_diaria = Contrato.ObtenerJornadaDiaria();
            decimal sueldo_por_dia = Contrato.ObtenerSueldoPorDia();

            if (jornada_diaria <= 0)
                return 0;

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
