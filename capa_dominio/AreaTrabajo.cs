using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class AreaTrabajo
    {
        public int AreaId { get; private set; }
        public string AreaNombre { get; private set; }
        public string Descripcion { get; private set; }

        public AreaTrabajo(int id, string nombre, string descripcion)
        {
            AreaId = id;
            AreaNombre = nombre;
            Descripcion = descripcion;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del área es obligatorio.");
        }
    }
}

