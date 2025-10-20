using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Cargo
    {
        public int CargoId { get; set; }
        public string Nombre { get; set; }
        public char Estado { get; set; } = 'A';
        public DateTime FechaCreacion { get; set; }

        public bool EstaActivo() => Estado == 'A';
    }
}