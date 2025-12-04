using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Bono
    {

        public int BonoId { get; set; }
        public Trabajador Trabajador { get; set; }
        public string BonoTipo { get; set; }
        public string BonoConcepto { get; set; }
        public decimal BonoMonto { get; set; }
        public DateTime BonoFecha { get; set; }
        public string BonoObservaciones { get; set; }


    }
}
