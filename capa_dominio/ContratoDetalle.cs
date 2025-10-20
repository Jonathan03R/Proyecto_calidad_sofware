using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ContratoDetalle
    {
        public int ContratoDetalleId { get; set; }
        public int ContratoId { get; set; }
        public int CambioContratoId { get; set; }
        public string Motivo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
        public DateTime FechaEvento { get; set; }

        public bool TieneCambios() => ValorAnterior != ValorActual;
    }
}
