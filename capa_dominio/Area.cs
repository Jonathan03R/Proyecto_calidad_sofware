using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Area
    {
        public int AreaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int SedeId { get; set; }
        public char Estado { get; set; } = 'A';
        public DateTime FechaCreacion { get; set; }

        public bool EstaActivo() => Estado == 'A';
    }
}
