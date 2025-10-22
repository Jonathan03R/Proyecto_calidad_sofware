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
        private DateTime faltaFecha;
        private string faltaTipo;
        private decimal faltaDias;
        private decimal faltaValorDiaNormal;
        private decimal faltaValorDescuento;
        private string faltaObservaciones;
        private string faltaDocumentoSoporte;

        
        public int FaltaId { get => faltaId; set => faltaId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public DateTime FaltaFecha { get => faltaFecha; set => faltaFecha = value; }
        public string FaltaTipo { get => faltaTipo; set => faltaTipo = value; }
        public decimal FaltaDias { get => faltaDias; set => faltaDias = value; }
        public decimal FaltaValorDiaNormal { get => faltaValorDiaNormal; set => faltaValorDiaNormal = value; }
        public decimal FaltaValorDescuento { get => faltaValorDescuento; set => faltaValorDescuento = value; }
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

        public decimal CalcularDescuento()
        {
            // Si la falta es injustificada, se descuenta el valor completo
            if (EsInjustificada())
                faltaValorDescuento = faltaValorDiaNormal * faltaDias;

            // Si es justificada o por enfermedad, no hay descuento
            else
                faltaValorDescuento = 0;

            return faltaValorDescuento;
        }

        public string Resumen()
        {
            return $"{faltaFecha.ToShortDateString()} - {faltaTipo} ({faltaDias} día/s) - Descuento: S/. {faltaValorDescuento}";
        }
    }
}
