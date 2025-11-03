using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Hijo
    {
        private int hijoId;
        private int trabajadorId;
        private string nombres;
        private string apellidos;
        private DateTime fechaNacimiento;
        private bool estudia;
        private bool tieneDiscapacidad;
        private char estado;
        private DateTime fechaCreacion;

        public int HijoId { get => hijoId; set => hijoId = value; }
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public string Nombres { get => nombres; set => nombres = value; }
        public string Apellidos { get => apellidos; set => apellidos = value; }
        public DateTime FechaNacimiento { get => fechaNacimiento; set => fechaNacimiento = value; }
        public bool Estudia { get => estudia; set => estudia = value; }
        public bool TieneDiscapacidad { get => tieneDiscapacidad; set => tieneDiscapacidad = value; }
        public char Estado { get => estado; set => estado = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
    }
}
