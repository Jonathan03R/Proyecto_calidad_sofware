using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class AdelantoSueldoDTO
    {
        private int adelantoId;
        private int trabajadorId;
        private decimal adelantoMonto;
        private DateTime adelantoFecha;
        private string adelantoMotivo;
        private string adelantoObservaciones;

        public int AdelantoId { get => adelantoId; set => adelantoId = value; }
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public decimal AdelantoMonto { get => adelantoMonto; set => adelantoMonto = value; }
        public DateTime AdelantoFecha { get => adelantoFecha; set => adelantoFecha = value; }
        public string AdelantoMotivo { get => adelantoMotivo; set => adelantoMotivo = value; }
        public string AdelantoObservaciones { get => adelantoObservaciones; set => adelantoObservaciones = value; }

    }
}
