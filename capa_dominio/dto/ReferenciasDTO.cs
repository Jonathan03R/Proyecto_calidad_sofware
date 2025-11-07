using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class AreaDTO
    {
        public int AreaId { get; set; }
        public string NombreArea { get; set; }
    }

    public class CargoDTO
    {
        public int CargoId { get; set; }
        public string NombreCargo { get; set; }
    }

    public class EstadoContratoDTO
    {
        public int EstadoId { get; set; }
        public string NombreEstado { get; set; }
    }

    public class TipoPensionDTO
    {
        public int TipoPensionId { get; set; }
        public string NombreTipo { get; set; }
    }

    public class TrabajadorDTO
    {
        public int TrabajadorId { get; set; }
        public string NombreCompleto { get; set; }
        // Puedes agregar más propiedades si luego las necesitas
        // Ej: DocumentoIdentidad, FechaIngreso, CargoId, etc.
    }
}
