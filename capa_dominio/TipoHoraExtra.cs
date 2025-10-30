using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoHoraExtra
    {
        
        private int tipoHoraExtraId;
        private string tiposHorasExtrasCodigo;
        private string tiposHorasExtrasNombre;
        private decimal tiposHorasExtrasMultiplicador;
        private char tiposHorasExtrasEstado;
        private DateTime tiposHorasExtrasFechaCreacion;

        
        public int TipoHoraExtraId { get => tipoHoraExtraId; set => tipoHoraExtraId = value; }
        public string TiposHorasExtrasCodigo { get => tiposHorasExtrasCodigo; set => tiposHorasExtrasCodigo = value; }
        public string TiposHorasExtrasNombre { get => tiposHorasExtrasNombre; set => tiposHorasExtrasNombre = value; }
        public decimal TiposHorasExtrasMultiplicador { get => tiposHorasExtrasMultiplicador; set => tiposHorasExtrasMultiplicador = value; }
        public char TiposHorasExtrasEstado { get => tiposHorasExtrasEstado; set => tiposHorasExtrasEstado = value; }
        public DateTime TiposHorasExtrasFechaCreacion { get => tiposHorasExtrasFechaCreacion; set => tiposHorasExtrasFechaCreacion = value; }

    }
}
