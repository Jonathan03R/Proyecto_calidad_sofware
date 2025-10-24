using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class TrabajadorProcesable
    {
        public int TrabajadorId { get; set; }
        public string TrabajadorCodigo { get; set; }
        public string TrabajadorEstado { get; set; }
        public int ContratoId { get; set; }
        public int TipoPensionId { get; set; }
        public int TipoSalarioId { get; set; }
        public int TipoJornadaId { get; set; }
        public int EstadoContratoId { get; set; }
        public DateTime ContratoFechaInicio { get; set; }
        public DateTime? ContratoFechaFin { get; set; }
    }
}
