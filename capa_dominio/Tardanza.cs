using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Tardanza
    {
        
        private int tardanzaId;
        private Trabajador trabajador;
        private DateTime tardanzaFecha;
        private int tardanzaMinutos;
        private decimal tardanzaHoras;
        private decimal tardanzaValorHoraNormal;
        private decimal tardanzaValorDescuento;
        private string tardanzaObservaciones;

       
        public int TardanzaId { get => tardanzaId; set => tardanzaId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public DateTime TardanzaFecha { get => tardanzaFecha; set => tardanzaFecha = value; }
        public int TardanzaMinutos { get => tardanzaMinutos; set => tardanzaMinutos = value; }
        public decimal TardanzaHoras { get => tardanzaHoras; set => tardanzaHoras = value; }
        public decimal TardanzaValorHoraNormal { get => tardanzaValorHoraNormal; set => tardanzaValorHoraNormal = value; }
        public decimal TardanzaValorDescuento { get => tardanzaValorDescuento; set => tardanzaValorDescuento = value; }
        public string TardanzaObservaciones { get => tardanzaObservaciones; set => tardanzaObservaciones = value; }

       
        public decimal CalcularDescuento()
        {
            // El descuento se calcula multiplicando el valor por hora por las horas de tardanza
            tardanzaValorDescuento = tardanzaValorHoraNormal * tardanzaHoras;
            return tardanzaValorDescuento;
        }

        public bool EsTardanzaLeve()
        {
            return tardanzaMinutos <= 15;
        }

        public bool EsTardanzaGrave()
        {
            return tardanzaMinutos > 60;
        }

        public string Resumen()
        {
            return $"{tardanzaFecha.ToShortDateString()} - {tardanzaMinutos} min ({tardanzaHoras} h) - Descuento: S/. {tardanzaValorDescuento}";
        }
    }
}
