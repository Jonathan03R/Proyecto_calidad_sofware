using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoIdentificacion
    {
        public int TipoIdentificacionId { get; set; }
        public string TipoIdentificacionNombre { get; set; }
        public string TipoIdentificacionValor { get; set; }
        public char TipoIdentificacionEstado { get; set; }
        public DateTime TipoIdentificacionFechaCreacion { get; set; }
    }
}
