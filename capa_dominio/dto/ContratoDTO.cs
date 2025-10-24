using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class ContratoDTO
    {
        private int? trabajadorId;
        private int? cargoId;
        private int? areaId;
        private int? tipoPensionId;
        private int? tipoSalarioId;
        private int? tipoJornadaId;
        private DateTime fechaInicio;
        private DateTime? fechaFin;
        private decimal? salario;
        private decimal? tarifaHora;
        private string modoPago;
        private string documentoUrl;
        private string descripcionFunciones;
        private string observaciones;

        public int? TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public int? CargoId { get => cargoId; set => cargoId = value; }
        public int? AreaId { get => areaId; set => areaId = value; }
        public int? TipoPensionId { get => tipoPensionId; set => tipoPensionId = value; }
        public int? TipoSalarioId { get => tipoSalarioId; set => tipoSalarioId = value; }
        public int? TipoJornadaId { get => tipoJornadaId; set => tipoJornadaId = value; }
        public DateTime FechaInicio { get => fechaInicio; set => fechaInicio = value; }
        public DateTime? FechaFin { get => fechaFin; set => fechaFin = value; }
        public decimal? Salario { get => salario; set => salario = value; }
        public decimal? TarifaHora { get => tarifaHora; set => tarifaHora = value; }
        public string ModoPago { get => modoPago; set => modoPago = value; }
        public string DocumentoUrl { get => documentoUrl; set => documentoUrl = value; }
        public string DescripcionFunciones { get => descripcionFunciones; set => descripcionFunciones = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }
    }
}
