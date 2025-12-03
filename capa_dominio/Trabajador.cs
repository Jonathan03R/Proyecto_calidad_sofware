using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Trabajador
    {
        public int TrabajadorId { get; set; }
        public string Codigo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public char Estado { get; set; }
        public List<HoraTrabajada> HorasTrabajadas { get; set; } = new List<HoraTrabajada>();
        public List<Contacto> Contactos { get; set; } = new List<Contacto>();
        public Contrato Contrato { get; set; }
        public List<Hijo> Hijos { get; set; } = new List<Hijo>();
        public string TrabajadorNombreCompleto { get; internal set; }

        public bool TieneDerechoAsignacionFamiliar()
        {
            System.Diagnostics.Debug.WriteLine(
                $"Verificando derecho a asignación familiar para trabajador ID: {TrabajadorId}");

            if (Hijos == null || Hijos.Count == 0)
            {
                return false;
            }

            return Hijos.Any(h =>
                h.Estado == 'A' &&
                (h.FechaNacimiento > DateTime.Now.AddYears(-18) ||
                 h.TieneDiscapacidad ||
                 h.Estudia));
        }
    }
}