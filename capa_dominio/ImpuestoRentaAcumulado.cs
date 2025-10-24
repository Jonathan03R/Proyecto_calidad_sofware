using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ImpuestoRentaAcumulado
    {
        
        private int impuestoRentaAcumuladoId;
        private Trabajador trabajador;
        private int impuestoRentaAcumuladoAnio;
        private decimal impuestoRentaRemuneracionBrutaAcumulada;
        private decimal impuestoRentaRetenidoAcumulado;
        private decimal impuestoRentaBaseImponibleAcumulada;
        private decimal impuestoRentaUitValor;
        private decimal impuestoRentaDeduccion7UIT;
        private DateTime impuestoRentaFechaUltimaActualizacion;

       
        public int ImpuestoRentaAcumuladoId { get => impuestoRentaAcumuladoId; set => impuestoRentaAcumuladoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public int ImpuestoRentaAcumuladoAnio { get => impuestoRentaAcumuladoAnio; set => impuestoRentaAcumuladoAnio = value; }
        public decimal ImpuestoRentaRemuneracionBrutaAcumulada { get => impuestoRentaRemuneracionBrutaAcumulada; set => impuestoRentaRemuneracionBrutaAcumulada = value; }
        public decimal ImpuestoRentaRetenidoAcumulado { get => impuestoRentaRetenidoAcumulado; set => impuestoRentaRetenidoAcumulado = value; }
        public decimal ImpuestoRentaBaseImponibleAcumulada { get => impuestoRentaBaseImponibleAcumulada; set => impuestoRentaBaseImponibleAcumulada = value; }
        public decimal ImpuestoRentaUitValor { get => impuestoRentaUitValor; set => impuestoRentaUitValor = value; }
        public decimal ImpuestoRentaDeduccion7UIT { get => impuestoRentaDeduccion7UIT; set => impuestoRentaDeduccion7UIT = value; }
        public DateTime ImpuestoRentaFechaUltimaActualizacion { get => impuestoRentaFechaUltimaActualizacion; set => impuestoRentaFechaUltimaActualizacion = value; }


        public decimal CalcularBaseImponibleActual()
        {
            decimal baseImponible = impuestoRentaRemuneracionBrutaAcumulada - impuestoRentaDeduccion7UIT;
            return baseImponible > 0 ? baseImponible : 0;
        }

    }
}
