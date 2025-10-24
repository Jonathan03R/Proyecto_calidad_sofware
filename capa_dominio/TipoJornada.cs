using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoJornada
    {
        
        private int tipoJornadaId;
        private string tipoJornadaNombre;
        private string tipoJornadaDescripcion;
        private char tipoJornadaEstado;
        private DateTime tipoJornadaFechaCreacion;

        
        public int TipoJornadaId { get => tipoJornadaId; set => tipoJornadaId = value; }
        public string TipoJornadaNombre { get => tipoJornadaNombre; set => tipoJornadaNombre = value; }
        public string TipoJornadaDescripcion { get => tipoJornadaDescripcion; set => tipoJornadaDescripcion = value; }
        public char TipoJornadaEstado { get => tipoJornadaEstado; set => tipoJornadaEstado = value; }
        public DateTime TipoJornadaFechaCreacion { get => tipoJornadaFechaCreacion; set => tipoJornadaFechaCreacion = value; }


        public bool EsTiempoCompleto()
        {
            return tipoJornadaNombre.ToLower().Contains("completo");
        }

        public bool EsMedioTiempo()
        {
            return tipoJornadaNombre.ToLower().Contains("medio");
        }

        public bool EsPorHoras()
        {
            return tipoJornadaNombre.ToLower().Contains("hora");
        }

    }
}
