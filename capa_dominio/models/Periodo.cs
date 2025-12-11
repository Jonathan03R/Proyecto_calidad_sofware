using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Periodo
    {
        public int PeriodoId { get; set; }
        public string PeriodoNombre { get; set; }
        public TipoSalario TipoSalario { get; set; }
        public DateTime PeriodoFechaInicio { get; set; }
        public DateTime PeriodoFechaFin { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; }

        public bool EsProcesado()
        {
            return EstadoId == 3;
        }
    }
}

