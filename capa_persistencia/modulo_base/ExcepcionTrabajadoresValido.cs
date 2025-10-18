using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_base
{
    public class ExcepcionTrabajador : Exception
    {
        public const string NO_EXISTE_REGISTRO = "No existe el trabajador.";
        public const string NO_EXISTEN_REGISTROS = "No existen trabajadores.";
        public const string ERROR_DE_CONSULTA = "No se pudo consultar el(los) trabajador(es), intente nuevamente o consulte con el administrador.";
        public const string ERROR_DE_CREACION = "No se pudo crear el trabajador, intente nuevamente o consulte con el administrador.";
        public const string ERROR_DE_ACTUALIZACION = "No se pudo modificar el trabajador, intente nuevamente o consulte con el administrador.";
        public const string ERROR_DE_ELIMINACION = "No se pudo eliminar el trabajador, intente nuevamente o consulte con el administrador.";
        public const string ERROR_DE_CANCELACION = "No se pudo cancelar la operación del trabajador, intente nuevamente o consulte con el administrador.";

        public ExcepcionTrabajador(string mensaje)
            : base(mensaje)
        {
        }
    }
}
