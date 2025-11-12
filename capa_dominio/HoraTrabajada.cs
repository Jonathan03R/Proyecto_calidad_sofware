using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio
{
    //public int BonoId { get => bonoId; set => bonoId = value; }
    public class HoraTrabajada
    {
        private DateTime fecha;
        private decimal horasNormales;
        private decimal horasExtras;
        private decimal horasDescanso;
        private decimal totalDiaTrabajado;
        private Contrato contrato;
        private List<TipoHoraExtra> TiposHorasExtras;

        public DateTime Fecha { get => fecha; set => fecha = value; }
        public decimal HorasNormales { get => horasNormales; set => horasNormales = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal HorasDescanso { get => horasDescanso; set => horasDescanso = value; }
        public decimal TotalDiaTrabajado { get => totalDiaTrabajado; set => totalDiaTrabajado = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public List<TipoHoraExtra> TiposHorasExtras1 { get => TiposHorasExtras; set => TiposHorasExtras = value; }


        public decimal CalcularPagoDia()
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo.");
            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
                throw new InvalidOperationException("No se cargaron los tipos de horas extras desde la base de datos.");

            decimal tarifaHora = Contrato.ContratoTarifaHora;
            decimal pagoNormal = HorasNormales * tarifaHora;
            decimal pagoExtras = 0;
            var dia = Fecha.DayOfWeek;

            decimal multPrimeras2 = ObtenerMultiplicador("PRIMERAS2");
            decimal multAdicionales = ObtenerMultiplicador("ADICIONALES");
            decimal multSabado = ObtenerMultiplicador("SABADO");
            decimal multDomingo = ObtenerMultiplicador("DOMINGO");

            if (dia == DayOfWeek.Saturday)
                return TotalDiaTrabajado = (HorasNormales + HorasExtras) * tarifaHora * multSabado;

            if (dia == DayOfWeek.Sunday)
                return TotalDiaTrabajado = (HorasNormales + HorasExtras) * tarifaHora * multDomingo;

            if (HorasExtras > 0)
            {
                decimal primerasDos = Math.Min(HorasExtras, 2);
                pagoExtras += primerasDos * tarifaHora * multPrimeras2;

                if (HorasExtras > 2)
                {
                    decimal adicionales = HorasExtras - 2;
                    pagoExtras += adicionales * tarifaHora * multAdicionales;
                }
            }

            TotalDiaTrabajado = pagoNormal + pagoExtras;
            return TotalDiaTrabajado;
        }

        private decimal ObtenerMultiplicador(string codigo)
        {
            var tipo = TiposHorasExtras.FirstOrDefault(t =>
                t.TiposHorasExtrasCodigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)
                && (t.TiposHorasExtrasEstado == 'A' || t.TiposHorasExtrasEstado.ToString() == "A"));

            if (tipo == null)
                throw new InvalidOperationException($"Falta el tipo de hora extra '{codigo}' (o está inactivo).");

            return tipo.TiposHorasExtrasMultiplicador;
        }
    }
}
