using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ImpuestoRentaTramo
    {
        public int ImpuestoRentaTramoId { get ; set ; }
        public int AnioVigencia { get ; set ; }
        public int NumeroTramo { get ; set ; }
        public decimal LimiteInferiorUIT { get ; set ; }
        public decimal? LimiteSuperiorUIT { get ; set ; }
        public decimal LimiteInferiorSoles { get ; set ; }
        public decimal? LimiteSuperiorSoles { get ; set ; }
        public decimal TasaPorcentaje { get ; set ; }
        public decimal AcumuladoAnteriorSoles { get ; set ; }

        public bool CorrespondeAlMonto(decimal baseImponibleUIT)
        {
            return baseImponibleUIT >= LimiteInferiorUIT && (LimiteSuperiorUIT == null || baseImponibleUIT <= LimiteSuperiorUIT);
        }
    }
}
