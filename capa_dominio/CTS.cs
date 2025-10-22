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
        private string ctsEstado;
        private string ctsObservaciones;

        public int CtsId { get => ctsId; set => ctsId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public string CtsSemestre { get => ctsSemestre; set => ctsSemestre = value; }
        public int CtsAnio { get => ctsAnio; set => ctsAnio = value; }
        public decimal CtsMonto { get => ctsMonto; set => ctsMonto = value; }
        public int CtsDiasTrabajados { get => ctsDiasTrabajados; set => ctsDiasTrabajados = value; }
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

        public void CalcularCTS(decimal sueldoBasico, decimal asignacionFamiliar, decimal promedioBonos)
        {
            
            if (CtsDiasTrabajados > 180)
                CtsDiasTrabajados = 180;
            if (CtsDiasTrabajados < 0)
                CtsDiasTrabajados = 0;

            CtsMonto = (sueldoBasico + asignacionFamiliar + promedioBonos) * (CtsDiasTrabajados / 180.0m);

            
            CtsMonto = Math.Round(CtsMonto, 2);

            
            CtsEstado = "Calculado";
            CtsObservaciones = $"CTS {CtsSemestre} {CtsAnio} - {CtsDiasTrabajados} días trabajados";
        }
    }
}


