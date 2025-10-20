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
        public string Codigo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public char Estado { get; set; } = 'A';

        // Relaciones
        public List<Contacto> Contactos { get; set; } = new List<Contacto>();
        public List<Contrato> Contratos { get; set; } = new List<Contrato>();

        // Métodos de negocio
        public bool EstaActivo() => Estado == 'A';
    }
}
