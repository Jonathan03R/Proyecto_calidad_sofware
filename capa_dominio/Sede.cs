using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Sede
    {
        public char SedeEstado { get; set; }
        public int SedeId { get; set; }
        public string SedeNombre { get; set; }
        public string SedeDireccion { get; set; }
        public string SedeDepartamento { get; set; }
        public string SedeProvincia { get; set; }
        public List<Area> Areas { get; set; } = new List<Area>();
    }
}

