using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Falta
    {
        
        public int FaltaId { get ; set; }
        public Trabajador Trabajador { get; set ; }
        public DateTime FaltaFecha { get ; set ; }
        public string FaltaTipo { get ; set ; }
        public decimal faltaDias { get ; set ; }
        public string FaltaObservaciones { get ; set ; }
        public string FaltaDocumentoSoporte { get ; set ; }

        
        public bool EsJustificada()
        {
            return FaltaTipo == "Justificada";
        }

        public bool EsInjustificada()
        {
            return FaltaTipo == "Injustificada";
        }

        public bool EsPorEnfermedad()
        {
            return FaltaTipo == "Enfermedad";
        }

    }
}
