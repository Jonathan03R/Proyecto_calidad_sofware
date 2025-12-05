using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ResumenKpisNominaDto
    {
        public int TotalPeriodosAbiertos { get; set; }
        public int TotalPeriodosProcesados { get; set; }
        public int TotalTrabajadoresInactivos { get; set; }
        public decimal TotalNetoGeneral { get; set; }
    }
}
