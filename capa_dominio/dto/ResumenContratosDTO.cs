using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ResumenContratosDto
    {
        public int TotalContratos { get; set; }
        public int ContratosActivos { get; set; }
        public int PorVencer30 { get; set; }
        public int AlertasLegales { get; set; }
    }
}
