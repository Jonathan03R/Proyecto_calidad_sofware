using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class AdelantoSueldo
    {
        
        private int adelantoId;
        private Trabajador trabajador;
        private decimal adelantoMonto;
        private DateTime adelantoFecha;
        private string adelantoMotivo;
        private string adelantoObservaciones;

        
        public int AdelantoId { get => adelantoId; set => adelantoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public decimal AdelantoMonto { get => adelantoMonto; set => adelantoMonto = value; }
        public DateTime AdelantoFecha { get => adelantoFecha; set => adelantoFecha = value; }
        public string AdelantoMotivo { get => adelantoMotivo; set => adelantoMotivo = value; }
        public string AdelantoObservaciones { get => adelantoObservaciones; set => adelantoObservaciones = value; }

        
        public bool EsMontoValido()
        {
            return adelantoMonto > 0;
        }

        public bool EsReciente()
        {
            return (DateTime.Now - adelantoFecha).TotalDays <= 30;
        }

        public string Resumen()
        {
            return $"{adelantoFecha.ToShortDateString()} - S/. {adelantoMonto} ({adelantoMotivo})";
        }
    }
}
