using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class CTS
    {
        
        private int ctsId;
        private Trabajador trabajador;
        private string ctsSemestre;
        private int ctsAnio;
        private decimal ctsMonto;
        private int ctsDiasTrabajados;
        private DateTime ctsFechaCalculo;
        private DateTime? ctsFechaDeposito;
        private string ctsEstado;
        private string ctsObservaciones;

       
        public int CtsId { get => ctsId; set => ctsId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public string CtsSemestre { get => ctsSemestre; set => ctsSemestre = value; }
        public int CtsAnio { get => ctsAnio; set => ctsAnio = value; }
        public decimal CtsMonto { get => ctsMonto; set => ctsMonto = value; }
        public int CtsDiasTrabajados { get => ctsDiasTrabajados; set => ctsDiasTrabajados = value; }
        public DateTime CtsFechaCalculo { get => ctsFechaCalculo; set => ctsFechaCalculo = value; }
        public DateTime? CtsFechaDeposito { get => ctsFechaDeposito; set => ctsFechaDeposito = value; }
        public string CtsEstado { get => ctsEstado; set => ctsEstado = value; }
        public string CtsObservaciones { get => ctsObservaciones; set => ctsObservaciones = value; }

        
        public bool EsPendiente()
        {
            return ctsEstado == "Pendiente";
        }

        public bool EstaDepositada()
        {
            return ctsEstado == "Depositada";
        }

        public void CalcularMonto(decimal sueldoBasico, decimal asignacionFamiliar, decimal promedioBonos)
        {
            // RN: La CTS se calcula proporcionalmente a los días trabajados (180 días = semestre completo)
            ctsMonto = (sueldoBasico + asignacionFamiliar + promedioBonos) * (ctsDiasTrabajados / 180m);
            ctsFechaCalculo = DateTime.Now;
        }

        public void RegistrarDeposito()
        {
            ctsEstado = "Depositada";
            ctsFechaDeposito = DateTime.Now;
        }

        public string Resumen()
        {
            return $"{ctsSemestre} {ctsAnio} - {trabajador.Persona.NombreCompleto()} - S/. {ctsMonto}";
        }
    }
}