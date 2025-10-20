using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Sede
    {
        public int SedeId { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public char Estado { get; set; } = 'A';

        public List<Area> Areas { get; set; } = new List<Area>();

        public bool EstaActiva() => Estado == 'A';
    }
}

