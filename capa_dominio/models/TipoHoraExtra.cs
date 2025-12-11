using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoHoraExtra
    {
        public int TipoHoraExtraId { get; set; }
        public string TiposHorasExtrasCodigo { get; set; }
        public string TiposHorasExtrasNombre { get; set; }
        public decimal TiposHorasExtrasMultiplicador { get; set; }
        public char TiposHorasExtrasEstado { get; set; }
    }
}
