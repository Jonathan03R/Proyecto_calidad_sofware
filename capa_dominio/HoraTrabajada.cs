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
            System.Diagnostics.Trace.WriteLine($"---- DIA {Fecha:yyyy-MM-dd} ----");

            if (Contrato == null)
                throw new InvalidOperationException("Contrato nulo.");
            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
                throw new InvalidOperationException("Tipos de horas extras no cargadas.");

            decimal tarifaHora = Contrato.ContratoTarifaHora;

            System.Diagnostics.Trace.WriteLine($"TarifaHora: {tarifaHora}");

            decimal pagoNormal = HorasNormales * tarifaHora;
            decimal pagoExtras = 0;

            System.Diagnostics.Trace.WriteLine($"HorasNormales: {HorasNormales} => PagoNormal: {pagoNormal}");

            var dia = Fecha.DayOfWeek;

            decimal multPrimeras2 = ObtenerMultiplicador("PRIMERAS2");
            decimal multAdicionales = ObtenerMultiplicador("ADICIONALES");
            decimal multSabado = ObtenerMultiplicador("SABADO");
            decimal multDomingo = ObtenerMultiplicador("DOMINGO");

            System.Diagnostics.Trace.WriteLine(
                $"Multiplicadores -> P2:{multPrimeras2} | ADI:{multAdicionales} | SAB:{multSabado} | DOM:{multDomingo}"
            );

            // sábado
            if (dia == DayOfWeek.Saturday)
            {
                TotalDiaTrabajado = (HorasNormales + HorasExtras) * tarifaHora * multSabado;
                System.Diagnostics.Trace.WriteLine($"SABADO => Total: {TotalDiaTrabajado}");
                return TotalDiaTrabajado;
            }

            // domingo
            if (dia == DayOfWeek.Sunday)
            {
                TotalDiaTrabajado = (HorasNormales + HorasExtras) * tarifaHora * multDomingo;
                System.Diagnostics.Trace.WriteLine($"DOMINGO => Total: {TotalDiaTrabajado}");
                return TotalDiaTrabajado;
            }

            // horas extras normales
            if (HorasExtras > 0)
            {
                decimal primerasDos = Math.Min(HorasExtras, 2);
                decimal pagoP2 = primerasDos * tarifaHora * multPrimeras2;

                System.Diagnostics.Trace.WriteLine($"Extras primeras 2: {primerasDos} => {pagoP2}");

                pagoExtras += pagoP2;

                if (HorasExtras > 2)
                {
                    decimal adicionales = HorasExtras - 2;
                    decimal pagoAdi = adicionales * tarifaHora * multAdicionales;

                    System.Diagnostics.Trace.WriteLine($"Extras adicionales: {adicionales} => {pagoAdi}");

                    pagoExtras += pagoAdi;
                }
            }

            TotalDiaTrabajado = pagoNormal + pagoExtras;

            System.Diagnostics.Trace.WriteLine(
                $"TOTAL DIA {Fecha:yyyy-MM-dd} => Normal:{pagoNormal} Extras:{pagoExtras} Total:{TotalDiaTrabajado}"
            );

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
