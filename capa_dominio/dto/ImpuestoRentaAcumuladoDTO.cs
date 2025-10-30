using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ImpuestoRentaAcumuladoDTO
    {
        public int TrabajadorId { get; set; }
        public int Anio { get; set; }
        public decimal RemuneracionBrutaAcumulada { get; set; }
        public decimal RetenidoAcumulado { get; set; }
        public decimal BaseImponibleAcumulada { get; set; }
        public decimal UitValor { get; set; }
        public decimal Deduccion7UIT { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
    }
}