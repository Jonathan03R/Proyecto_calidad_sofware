using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoPension
    {
        public int TipoPensionId { get; set; }
        public string Nombre { get; set; }
        public string Entidad { get; set; } // "ONP" o "AFP"
        public char Estado { get; set; } = 'A';
        public DateTime FechaCreacion { get; set; }

        // Parámetros para AFP (si aplica)
        public decimal PorcentajeComisionFlujo { get; set; } = 0m;  // e.g. 0.0175m
        public decimal PorcentajeComisionSaldo { get; set; } = 0m;  // e.g. 0.00m si no aplica
        public decimal PorcentajeSeguro { get; set; } = 0m;         // e.g. 0.0055m

        public bool EstaActivo() => Estado == 'A';

    }
}