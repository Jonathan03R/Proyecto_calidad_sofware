using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Empleado
    {
        public int TrabajadorId { get; private set; }
        public string CodigoTrabajador { get; private set; }
        public string Nombres { get; private set; }
        public string Apellidos { get; private set; }
        public string TipoIdentificacion { get; private set; }
        public string NumeroIdentificacion { get; private set; }
        public string Estado { get; private set; }

        public Empleado(int trabajadorId, string codigo, string nombres, string apellidos,
            string tipoIdentificacion, string numeroIdentificacion, string estado = "A")
        {
            TrabajadorId = trabajadorId;
            CodigoTrabajador = codigo;
            Nombres = nombres;
            Apellidos = apellidos;
            TipoIdentificacion = tipoIdentificacion;
            NumeroIdentificacion = numeroIdentificacion;
            Estado = estado;

            ValidarEmpleado();
        }

        public void ValidarEmpleado()
        {
            if (string.IsNullOrWhiteSpace(CodigoTrabajador))
                throw new ArgumentException("El código del trabajador es obligatorio.");
            if (string.IsNullOrWhiteSpace(Nombres))
                throw new ArgumentException("El nombre del trabajador es obligatorio.");
            if (string.IsNullOrWhiteSpace(Apellidos))
                throw new ArgumentException("El apellido del trabajador es obligatorio.");
            if (string.IsNullOrWhiteSpace(TipoIdentificacion))
                throw new ArgumentException("El tipo de identificación es obligatorio.");
            if (string.IsNullOrWhiteSpace(NumeroIdentificacion))
                throw new ArgumentException("El número de identificación es obligatorio.");
        }

        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    }
}
