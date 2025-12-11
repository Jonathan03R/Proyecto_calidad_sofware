using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class DetalleParametro
    {

        public int detalleparametroid { get; set; }
        public Parametro parametro { get; set; }
        public Trabajador trabajador { get; set; }
        public decimal detalleparametrovalor { get; set; }
        public DateTime detalleparametroaplicadesde { get; set; }
        public DateTime? detalleparametroaplicahasta { get; set; }
        public string detalleparametroobservaciones { get; set; }

        public bool estavigente()
        {
            var hoy = DateTime.Now;
            return detalleparametroaplicadesde <= hoy &&
                   (!detalleparametroaplicahasta.HasValue || detalleparametroaplicahasta.Value >= hoy);
        }

    }
}
