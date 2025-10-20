using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Cargo
    {
        public int CargoId { get; private set; }
        public string CargoNombre { get; private set; }

        public Cargo(int id, string nombre)
        {
            CargoId = id;
            CargoNombre = nombre;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del cargo es obligatorio.");
        }
    }
}
