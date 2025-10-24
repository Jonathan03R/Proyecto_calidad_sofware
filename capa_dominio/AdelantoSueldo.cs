using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{


    public class AdelantoSueldo
    {

        private int adelantoId;
        private Trabajador trabajador;
        private Periodo periodo;
        private decimal adelantoMonto;
        private DateTime adelantoFecha;
        private string adelantoMotivo;
        private string adelantoObservaciones;


        public int AdelantoId { get => adelantoId; set => adelantoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public Periodo Periodo { get => periodo; set => periodo = value; }
        public decimal AdelantoMonto { get => adelantoMonto; set => adelantoMonto = value; }
        public DateTime AdelantoFecha { get => adelantoFecha; set => adelantoFecha = value; }
        public string AdelantoMotivo { get => adelantoMotivo; set => adelantoMotivo = value; }
        public string AdelantoObservaciones { get => adelantoObservaciones; set => adelantoObservaciones = value; }


        public bool EsMontoValido()
        {
            return adelantoMonto > 0;
        }

        public bool EsPeriodoActual()
        {
            return adelantoFecha >= Periodo.PeriodoFechaInicio
                && adelantoFecha <= Periodo.PeriodoFechaFin;
        }



    }
}
