using System;
using System.Collections.Generic;

namespace capa_dominio.dto
{
    public class PersonaSinContratoDTO
    {
        public int TrabajadorId { get; set; }
        public string PersonaApellido { get; set; }
        public string PersonaNombre { get; set; }
        public string PersonaIdentificacion { get; set; }
        public string EstadoContrato { get; set; }
    }
}