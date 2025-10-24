using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Falta
    {
       
        private int faltaId;
        private Trabajador trabajador;
        private DateTime faltaFecha; // 24-10-2025
        private string faltaTipo;
        private decimal faltaDiasNombres; // 3 dias
        private string faltaObservaciones;
        private string faltaDocumentoSoporte;

        
        public int FaltaId { get => faltaId; set => faltaId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public DateTime FaltaFecha { get => faltaFecha; set => faltaFecha = value; }
        public string FaltaTipo { get => faltaTipo; set => faltaTipo = value; }
        public decimal faltaDias { get => faltaDiasNombres; set => faltaDiasNombres = value; }
        public string FaltaObservaciones { get => faltaObservaciones; set => faltaObservaciones = value; }
        public string FaltaDocumentoSoporte { get => faltaDocumentoSoporte; set => faltaDocumentoSoporte = value; }

        
        public bool EsJustificada()
        {
            return faltaTipo == "Justificada";
        }

        public bool EsInjustificada()
        {
            return faltaTipo == "Injustificada";
        }

        public bool EsPorEnfermedad()
        {
            return faltaTipo == "Enfermedad";
        }

    }
}
