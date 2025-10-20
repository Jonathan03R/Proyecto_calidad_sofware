using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Trabajador
    {
        public int TrabajadorId { get; set; }
        public int PersonaId { get; set; }
        public string Codigo { get; set; }
        public char Estado { get; set; } = 'A';
        public char EstadoReporte { get; set; } = 'A';
        public DateTime FechaCreacion { get; set; }

        public bool EstaActivo() => Estado == 'A';
        public bool EstaInactivo() => Estado == 'I';
    }
}
