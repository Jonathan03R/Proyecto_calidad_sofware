using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Periodo
    {
        private int periodoId;
        private string periodoNombre;
        private TipoSalario tipoSalario; 
        private DateTime periodoFechaInicio;
        private DateTime periodoFechaFin;
        private int estadoId;
        private string estadoNombre;

        
        public int PeriodoId { get => periodoId; set => periodoId = value; }
        public string PeriodoNombre { get => periodoNombre; set => periodoNombre = value; }
        public TipoSalario TipoSalario { get => tipoSalario; set => tipoSalario = value; }
        public DateTime PeriodoFechaInicio { get => periodoFechaInicio; set => periodoFechaInicio = value; }
        public DateTime PeriodoFechaFin { get => periodoFechaFin; set => periodoFechaFin = value; }
        public int EstadoId { get => estadoId; set => estadoId = value; }
        public string EstadoNombre { get => estadoNombre; set => estadoNombre = value; }

        public bool EsProcesado()
        {
            return estadoId == 3;
        }


    }
}

