using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Hijo
    {
        public int HijoId { get; set; }
        public int TrabajadorId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Estudia { get; set; }
        public bool TieneDiscapacidad { get; set ; }
        public char Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
