using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ResumenNominaDTO
    {
        public string PeriodoNombre { get; set; }
        public DateTime NominaFechaProcesamiento { get; set; }
        public int NominaTotalEmpleados { get; set; }
        public string NominaEstado { get; set; }
        public decimal NominaTotalNeto { get; set; }
    }
}
