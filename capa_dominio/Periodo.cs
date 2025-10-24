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
        private string periodoEstado;

        
        public int PeriodoId { get => periodoId; set => periodoId = value; }
        public string PeriodoNombre { get => periodoNombre; set => periodoNombre = value; }
        public TipoSalario TipoSalario { get => tipoSalario; set => tipoSalario = value; }
        public DateTime PeriodoFechaInicio { get => periodoFechaInicio; set => periodoFechaInicio = value; }
        public DateTime PeriodoFechaFin { get => periodoFechaFin; set => periodoFechaFin = value; }
        public string PeriodoEstado { get => periodoEstado; set => periodoEstado = value; }

        
        public bool EstaProcesado()
        {
            return periodoEstado == "Procesado";
        }

        public bool EsActivo()
        {
            return periodoEstado == "Activo";
        }

        public bool EsPeriodoActual()
        {
            DateTime today = DateTime.Today;

            return today >= this.periodoFechaInicio.Date && today <= this.periodoFechaFin.Date;
        }

    }
}

