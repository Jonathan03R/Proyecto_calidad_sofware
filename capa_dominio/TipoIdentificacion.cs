using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoIdentificacion
    {
        
        private int tipoIdentificacionId;
        private string tipoIdentificacionNombre;
        private string tipoIdentificacionValor;
        private char tipoIdentificacionEstado;
        private DateTime tipoIdentificacionFechaCreacion;

        
        public int TipoIdentificacionId { get => tipoIdentificacionId; set => tipoIdentificacionId = value; }
        public string TipoIdentificacionNombre { get => tipoIdentificacionNombre; set => tipoIdentificacionNombre = value; }
        public string TipoIdentificacionValor { get => tipoIdentificacionValor; set => tipoIdentificacionValor = value; }
        public char TipoIdentificacionEstado { get => tipoIdentificacionEstado; set => tipoIdentificacionEstado = value; }
        public DateTime TipoIdentificacionFechaCreacion { get => tipoIdentificacionFechaCreacion; set => tipoIdentificacionFechaCreacion = value; }

        
        public bool EsActivo()
        {
            return tipoIdentificacionEstado == 'A';
        }

        public bool EsValido()
        {
            return !string.IsNullOrEmpty(tipoIdentificacionValor);
        }
    }
}
