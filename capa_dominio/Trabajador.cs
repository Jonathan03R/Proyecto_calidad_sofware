using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Trabajador
    {
        
        private int trabajadorId;
        private Persona persona; 
        private string trabajadorCodigo;
        private char trabajadorEstado;
        private DateTime trabajadorFechaCreacion;

        
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public Persona Persona { get => persona; set => persona = value; }
        public string TrabajadorCodigo { get => trabajadorCodigo; set => trabajadorCodigo = value; }
        public char TrabajadorEstado { get => trabajadorEstado; set => trabajadorEstado = value; }
        public DateTime TrabajadorFechaCreacion { get => trabajadorFechaCreacion; set => trabajadorFechaCreacion = value; }

        
        public bool EsActivo()
        {
            return trabajadorEstado == 'A';
        }

        public bool EsInactivo()
        {
            return trabajadorEstado == 'I';
        }

        public bool CodigoValido()
        {
            return !string.IsNullOrEmpty(trabajadorCodigo) && trabajadorCodigo.Length == 8;
        }
    }
}