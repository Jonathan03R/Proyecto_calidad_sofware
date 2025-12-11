using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoSalario
    {
        public int TipoSalarioId { get; set; }
        public string TipoSalarioNombre { get; set; }
        public char TipoSalarioEstado { get; set; }
        public DateTime TipoSalarioFechaCreacion { get; set; }
    }
}



