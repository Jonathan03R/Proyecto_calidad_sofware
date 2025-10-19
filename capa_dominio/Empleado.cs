using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Empleado : DominioBase
    {
        public int TrabajadorId { get; set; }
        public string CodigoTrabajador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Prefijo { get; set; }
        public string Identificacion { get; set; }
        public char Estado { get; set; }

        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

        public void Activar() => Estado = 'A';
        public void Inactivar() => Estado = 'I';

        public void ValidarDatosBasicos()
        {
            ValidarCampoObligatorio("Nombres", Nombres);
            ValidarCampoObligatorio("Apellidos", Apellidos);
            ValidarCampoObligatorio("Identificación", Identificacion);
        }

        public override string ToString()
        {
            return $"{CodigoTrabajador} - {NombreCompleto}";
        }
    }
}
