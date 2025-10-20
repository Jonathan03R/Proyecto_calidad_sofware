using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoSalario
    {
        public int TipoSalarioId { get; private set; }
        public string Nombre { get; private set; }

        public TipoSalario(int id, string nombre)
        {
            TipoSalarioId = id;
            Nombre = nombre;

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del tipo de salario es obligatorio.");
        }
    }
}

