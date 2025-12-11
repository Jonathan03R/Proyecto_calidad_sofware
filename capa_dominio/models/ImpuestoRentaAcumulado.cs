using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ImpuestoRentaAcumulado
    {    
        public int ImpuestoRentaAcumuladoId { get ; set ; }
        public Trabajador Trabajador { get ; set ; }
        public int ImpuestoRentaAcumuladoAnio { get ; set ; }
        public decimal ImpuestoRentaRemuneracionBrutaAcumulada { get ; set ; }
        public decimal ImpuestoRentaRetenidoAcumulado { get ; set ; }
        public decimal ImpuestoRentaBaseImponibleAcumulada { get ; set ; }
        public decimal ImpuestoRentaUitValor { get ; set ; }
        public decimal ImpuestoRentaDeduccion7UIT { get ; set ; }
        public DateTime ImpuestoRentaFechaUltimaActualizacion { get ; set ; }


        public decimal CalcularBaseImponibleActual()
        {
            decimal baseImponible = ImpuestoRentaRemuneracionBrutaAcumulada - ImpuestoRentaDeduccion7UIT;
            return baseImponible > 0 ? baseImponible : 0;
        }

    }
}
