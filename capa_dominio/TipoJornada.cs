using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoJornada
    {
        public int TipoJornadaId { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }

        public TipoJornada(int id, string nombre, string descripcion)
        {
            TipoJornadaId = id;
            Nombre = nombre;
            Descripcion = descripcion;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la jornada es obligatorio.");
        }
    }
}

