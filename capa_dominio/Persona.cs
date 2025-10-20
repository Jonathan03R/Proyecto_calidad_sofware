using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Persona
    {

        private int personaId;
        private string personaNombre;
        private string personaApellido;
        private TipoIdentificacion tipoIdentificacion; 
        private string personaIdentificacion;
        private char personaEstado;
        private DateTime personaFechaCreacion;

        
        public int PersonaId { get => personaId; set => personaId = value; }
        public string PersonaNombre { get => personaNombre; set => personaNombre = value; }
        public string PersonaApellido { get => personaApellido; set => personaApellido = value; }
        public TipoIdentificacion TipoIdentificacion { get => tipoIdentificacion; set => tipoIdentificacion = value; }
        public string PersonaIdentificacion { get => personaIdentificacion; set => personaIdentificacion = value; }
        public char PersonaEstado { get => personaEstado; set => personaEstado = value; }
        public DateTime PersonaFechaCreacion { get => personaFechaCreacion; set => personaFechaCreacion = value; }

       
        public bool EsActivo()
        {
            return personaEstado == 'A';
        }

        public bool IdentificacionValida()
        {
            return !string.IsNullOrEmpty(personaIdentificacion) && personaIdentificacion.Length >= 8;
        }

        public string NombreCompleto()
        {
            return $"{personaNombre} {personaApellido}";
        }
    }
}
