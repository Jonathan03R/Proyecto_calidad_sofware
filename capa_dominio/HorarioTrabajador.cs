using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class HorarioTrabajo
    {
        public int HorarioTrabajoId { get; set; }
        public string Nombre { get; set; }
        public TimeSpan Entrada { get; set; }
        public TimeSpan Salida { get; set; }
        public char Estado { get; set; } = 'A';

        public bool EstaActivo() => Estado == 'A';
    }
}
