using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{

    /// <summary>
    ///revisar si hay proc para traer contactos de un trabajador
    /// </summary>
    public class Contacto
    {
        private int contactoId;
        private string telefono;
        private string email;
        private string direccion;
        private char estado = 'A';

        public int ContactoId { get => contactoId; set => contactoId = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Email { get => email; set => email = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public char Estado { get => estado; set => estado = value; }

        public bool EstaActivo() => Estado == 'A';
    }
}
