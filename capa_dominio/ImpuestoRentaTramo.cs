using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ImpuestoRentaTramo
    {
       
        private int impuestoRentaTramoId;
        private int impuestoRentaTramoAnioVigencia;
        private int impuestoRentaTramoNumero;
        private decimal impuestoRentaTramoLimiteInferiorUIT;
        private decimal impuestoRentaTramoLimiteSuperiorUIT;
        private decimal impuestoRentaTramoLimiteInferiorSoles;
        private decimal impuestoRentaTramoLimiteSuperiorSoles;
        private decimal impuestoRentaTramoTasaPorcentaje;
        private decimal impuestoRentaTramoAcumuladoAnteriorSoles;

        
        public int ImpuestoRentaTramoId { get => impuestoRentaTramoId; set => impuestoRentaTramoId = value; }
        public int ImpuestoRentaTramoAnioVigencia { get => impuestoRentaTramoAnioVigencia; set => impuestoRentaTramoAnioVigencia = value; }
        public int ImpuestoRentaTramoNumero { get => impuestoRentaTramoNumero; set => impuestoRentaTramoNumero = value; }
        public decimal ImpuestoRentaTramoLimiteInferiorUIT { get => impuestoRentaTramoLimiteInferiorUIT; set => impuestoRentaTramoLimiteInferiorUIT = value; }
        public decimal ImpuestoRentaTramoLimiteSuperiorUIT { get => impuestoRentaTramoLimiteSuperiorUIT; set => impuestoRentaTramoLimiteSuperiorUIT = value; }
        public decimal ImpuestoRentaTramoLimiteInferiorSoles { get => impuestoRentaTramoLimiteInferiorSoles; set => impuestoRentaTramoLimiteInferiorSoles = value; }
        public decimal ImpuestoRentaTramoLimiteSuperiorSoles { get => impuestoRentaTramoLimiteSuperiorSoles; set => impuestoRentaTramoLimiteSuperiorSoles = value; }
        public decimal ImpuestoRentaTramoTasaPorcentaje { get => impuestoRentaTramoTasaPorcentaje; set => impuestoRentaTramoTasaPorcentaje = value; }
        public decimal ImpuestoRentaTramoAcumuladoAnteriorSoles { get => impuestoRentaTramoAcumuladoAnteriorSoles; set => impuestoRentaTramoAcumuladoAnteriorSoles = value; }

       
        public bool CorrespondeAlMonto(decimal baseImponible)
        {
            return baseImponible >= impuestoRentaTramoLimiteInferiorSoles &&
                   baseImponible <= impuestoRentaTramoLimiteSuperiorSoles;
        }

        public decimal CalcularImpuesto(decimal baseImponible)
        {
            if (baseImponible < impuestoRentaTramoLimiteInferiorSoles)
                return 0;

            decimal exceso = Math.Min(baseImponible, impuestoRentaTramoLimiteSuperiorSoles) - impuestoRentaTramoLimiteInferiorSoles;
            decimal impuesto = (exceso * (impuestoRentaTramoTasaPorcentaje / 100)) + impuestoRentaTramoAcumuladoAnteriorSoles;

            return Math.Round(impuesto, 2);
        }

        public string Resumen()
        {
            return $"Tramo {impuestoRentaTramoNumero}: {impuestoRentaTramoLimiteInferiorSoles} - {impuestoRentaTramoLimiteSuperiorSoles} " +
                   $"({impuestoRentaTramoTasaPorcentaje}%)";
        }
    }
}
