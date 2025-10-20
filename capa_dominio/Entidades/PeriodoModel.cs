using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.Entidades
{
    public class PeriodoModel
    {
        public int periodo_id { get; set; }
        public string periodo_nombre { get; set; }
        public string TipoPago { get; set; }
        public DateTime periodo_fecha_inicio { get; set; }
        public DateTime periodo_fecha_fin { get; set; }
        public string periodo_estado { get; set; }

        public bool EsPeriodoValido(int periodoID)
        {
            return periodoID > 0;

        }
    }
}
