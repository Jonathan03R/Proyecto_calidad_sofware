using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_base
{
    public class NominaException : Exception
    {
        public const string ERROR_DE_CREACION = "ERROR_DE_CREACION";
        public const string ERROR_DE_ACTUALIZACION = "ERROR_DE_ACTUALIZACION";
        public const string ERROR_DE_CONSULTA = "ERROR_DE_CONSULTA";

        public string Codigo { get; }

        public NominaException(string codigo, string detalle = null)
            : base(detalle ?? codigo)
        {
            Codigo = codigo;
        }
    }
}
