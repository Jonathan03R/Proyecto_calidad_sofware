using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Parametro
    {
        public int ParametroId { get; set; }
        public string ParametroCodigo { get; set; }
        public string ParametroNombre { get; set; }
        public decimal ParametroValor { get; set; }
        public DateTime ParametroFechaVigencia { get; set; }
        public string ParametroEstado { get; set; }

        public bool EsVigente()
        {
            return ParametroEstado == "Activo" && ParametroFechaVigencia <= DateTime.Now;
        }
    }
}

