using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class CambioContrato
    {
        public int CambioContratoId { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public char Estado { get; set; } = 'A';
        public DateTime FechaCreacion { get; set; }

        public bool EstaActivo() => Estado == 'A';
    }
}