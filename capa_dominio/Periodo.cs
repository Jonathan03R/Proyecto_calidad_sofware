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
        private string periodoEstado;

        public int PeriodoId { get => periodoId; set => periodoId = value; }
        public string PeriodoNombre { get => periodoNombre; set => periodoNombre = value; }
        public TipoSalario TipoSalario { get => tipoSalario; set => tipoSalario = value; }
        public string PeriodoEstado { get => periodoEstado; set => periodoEstado = value; }
    }
}
