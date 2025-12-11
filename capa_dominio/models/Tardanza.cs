using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Tardanza
    {
        public int TardanzaId { get; set; }
        public Trabajador Trabajador { get; set; }
        public DateTime TardanzaFecha { get; set; }
        public int TardanzaMinutos { get; set; }
        public decimal TardanzaHoras { get; set; }
        public decimal TardanzaValorHoraNormal { get; set; }
        public decimal TardanzaValorDescuento { get; set; }
        public string TardanzaObservaciones { get; set; }
    }
}