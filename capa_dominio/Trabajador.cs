using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Trabajador
    {
        public static int trabajadorID;
        private int trabajadorId;
        private string codigo;
        private string nombres;
        private string apellidos;
        private string tipoIdentificacion;
        private string identificacion;
        private char estado;
        private List<HoraTrabajada> horasTrabajadas = new List<HoraTrabajada>();
        private List<Contacto> contactos = new List<Contacto>();
        private Contrato contrato;
        private List<Hijo> hijos;

        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }

        public string Codigo { get => codigo; set => codigo = value; }

        public string Nombres { get => nombres; set => nombres = value; }

        public string Apellidos { get => apellidos; set => apellidos = value; }

        public string TipoIdentificacion { get => tipoIdentificacion; set => tipoIdentificacion = value; }

        public string Identificacion { get => identificacion; set => identificacion = value; }

        public char Estado { get => estado; set => estado = value; }

        public List<Contacto> Contactos { get => contactos; set => contactos = value; }

        public Contrato Contrato { get => contrato; set => contrato = value; }

        //public List<Contrato> Contratos { get => contratos; set => contratos = value; }

        public List<HoraTrabajada> HorasTrabajadas { get => horasTrabajadas; set => horasTrabajadas = value; }

        public List<Hijo> Hijos { get => hijos; set => hijos = value; } 
        public string TrabajadorNombreCompleto { get; internal set; }

        // Según la ley peruana, tiene derecho si tiene al menos un hijo menor de 18
        // o mayor de edad con discapacidad o estudiante dependiente
        public bool TieneDerechoAsignacionFamiliar()
        {
            System.Diagnostics.Debug.WriteLine($"Verificando derecho a asignación familiar para trabajador ID: {trabajadorId}");
            return hijos.Any(h =>
                h.Estado == 'a' &&
                (h.FechaNacimiento > DateTime.Now.AddYears(-18) ||
                 h.TieneDiscapacidad ||
                 h.Estudia));
        }
    }
}