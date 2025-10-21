using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Bono
    {
        
        private int bonoId;
        private Trabajador trabajador;
        private string bonoTipo;
        private string bonoConcepto;
        private decimal bonoMonto;
        private DateTime bonoFecha;
        private string bonoObservaciones;

        
        public int BonoId { get => bonoId; set => bonoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public string BonoTipo { get => bonoTipo; set => bonoTipo = value; }
        public string BonoConcepto { get => bonoConcepto; set => bonoConcepto = value; }
        public decimal BonoMonto { get => bonoMonto; set => bonoMonto = value; }
        public DateTime BonoFecha { get => bonoFecha; set => bonoFecha = value; }
        public string BonoObservaciones { get => bonoObservaciones; set => bonoObservaciones = value; }

        
        public bool EsRegular()
        {
            return bonoTipo == "Regular";
        }

        public bool EsEspecial()
        {
            return bonoTipo == "Especial";
        }

        public bool EsRendimiento()
        {
            return bonoTipo == "Rendimiento";
        }

        public bool MontoValido()
        {
            return bonoMonto > 0;
        }

        public string Resumen()
        {
            return $"{bonoFecha.ToShortDateString()} - {bonoTipo}: {bonoConcepto} (S/. {bonoMonto})";
        }
    }
}
