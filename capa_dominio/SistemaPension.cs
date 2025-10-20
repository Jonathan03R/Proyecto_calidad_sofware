using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class SistemaPension
    {
        public int TipoPensionId { get; private set; }
        public string Nombre { get; private set; }
        public string Entidad { get; private set; }

        public SistemaPension(int id, string nombre, string entidad)
        {
            TipoPensionId = id;
            Nombre = nombre;
            Entidad = entidad;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del sistema de pensión es obligatorio.");
        }
    }
}
