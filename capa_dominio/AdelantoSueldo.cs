using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class AdelantoSueldo
    {

        public int AdelantoId { get; set; }
        public Trabajador Trabajador { get; set; }
        public Periodo Periodo { get; set; }
        public decimal AdelantoMonto { get; set; }
        public DateTime AdelantoFecha { get; set; }
        public string AdelantoMotivo { get; set; }
        public string AdelantoObservaciones { get; set; }

    }
}
