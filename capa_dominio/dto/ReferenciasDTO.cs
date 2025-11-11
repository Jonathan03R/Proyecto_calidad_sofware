using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    /// <summary>
    /// DTO para Áreas/Sedes
    /// </summary>
    public class AreaDTO
    {
        public int AreaId { get; set; }
        public string NombreArea { get; set; }
    }

    /// <summary>
    /// DTO para Cargos
    /// </summary>
    public class CargoDTO
    {
        public int CargoId { get; set; }
        public string NombreCargo { get; set; }
    }

    /// <summary>
    /// DTO para Estados de Contrato
    /// </summary>
    public class EstadoContratoDTO
    {
        public int EstadoId { get; set; }
        public string NombreEstado { get; set; }
    }

    /// <summary>
    /// DTO para Tipos de Pensión
    /// </summary>
    public class TipoPensionDTO
    {
        public int TipoPensionId { get; set; }
        public string NombreTipo { get; set; }
    }

    /// <summary>
    /// DTO para Tipos de Salario
    /// </summary>
    public class TipoSalarioDTO
    {
        public int TipoSalarioId { get; set; }
        public string NombreTipo { get; set; }
    }

    /// <summary>
    /// DTO para Tipos de Jornada
    /// </summary>
    public class TipoJornadaDTO
    {
        public int TipoJornadaId { get; set; }
        public string NombreTipo { get; set; }
    }

    /// <summary>
    /// DTO para Trabajadores (vista simplificada)
    /// </summary>
    public class TrabajadorDTO
    {
        public int TrabajadorId { get; set; }
        public string NombreCompleto { get; set; }
    }
}