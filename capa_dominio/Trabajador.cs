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
        private List<Contacto> contactos = new List<Contacto>();
        private List<Contrato> contratos = new List<Contrato>();
            
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }

        public string Codigo { get => codigo; set => codigo = value; }

        public string Nombres { get => nombres; set => nombres = value; }

        public string Apellidos { get => apellidos; set => apellidos = value; }

        public string TipoIdentificacion { get => tipoIdentificacion; set => tipoIdentificacion = value; }

        public string Identificacion { get => identificacion; set => identificacion = value; }

        public char Estado { get => estado; set => estado = value; }

        public List<Contacto> Contactos { get => contactos; set => contactos = value; }

        public List<Contrato> Contratos { get => contratos; set => contratos = value; }
        public string TrabajadorNombreCompleto { get; internal set; }

        // Métodos de negocio
        public bool EstaActivo() => Estado == 'A';
    }
}