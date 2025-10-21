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
        private int anioVigencia;
        private int numeroTramo;
        private decimal limiteInferiorUIT;
        private decimal limiteSuperiorUIT;
        private decimal tasaPorcentaje;

        public int ImpuestoRentaTramoId { get => impuestoRentaTramoId; set => impuestoRentaTramoId = value; }
        public int AnioVigencia { get => anioVigencia; set => anioVigencia = value; }
        public int NumeroTramo { get => numeroTramo; set => numeroTramo = value; }
        public decimal LimiteInferiorUIT { get => limiteInferiorUIT; set => limiteInferiorUIT = value; }
        public decimal LimiteSuperiorUIT { get => limiteSuperiorUIT; set => limiteSuperiorUIT = value; }
        public decimal TasaPorcentaje { get => tasaPorcentaje; set => tasaPorcentaje = value; }

        public bool CorrespondeAlMonto(decimal baseImponibleUIT)
        {
            return baseImponibleUIT >= limiteInferiorUIT && baseImponibleUIT <= limiteSuperiorUIT;
        }


        
    }
}
