using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoJornada
    {

        public int TipoJornadaId { get; set; }
        public string TipoJornadaNombre { get; set; }
        public string TipoJornadaDescripcion { get; set; }
        public char TipoJornadaEstado { get; set; }
        public DateTime TipoJornadaFechaCreacion { get; set; }

        public bool EsTiempoCompleto()
        {
            return TipoJornadaNombre.ToLower().Contains("completo");
        }

        public bool EsMedioTiempo()
        {
            return TipoJornadaNombre.ToLower().Contains("medio");
        }

        public bool EsPorHoras()
        {
            return TipoJornadaNombre.ToLower().Contains("hora");
        }

    }
}
