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
        private Contrato contrato;
        private List<Contacto> contactos = new List<Contacto>();
        private HoraTrabajada horaTrabajada;

        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }

        public string Codigo { get => codigo; set => codigo = value; }

        public string Nombres { get => nombres; set => nombres = value; }

        public string Apellidos { get => apellidos; set => apellidos = value; }

        public string TipoIdentificacion { get => tipoIdentificacion; set => tipoIdentificacion = value; }

        public string Identificacion { get => identificacion; set => identificacion = value; }

        public char Estado { get => estado; set => estado = value; }

        public List<Contacto> Contactos { get => contactos; set => contactos = value; }

        public HoraTrabajada HoraTrabajada { get => HoraTrabajada; set => HoraTrabajada = value; }
        public string TrabajadorNombreCompleto { get; internal set; }
        public Contrato Contrato { get => Contrato; set => Contrato = value; }

        // Métodos de negocio
        public bool EstaActivo() => Estado == 'A';
    }
}