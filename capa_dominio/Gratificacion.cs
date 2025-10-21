using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Gratificacion
    {
        private int gratificacionId;
        private Trabajador trabajador;
        private string gratificacionSemestre;
        private int gratificacionAnio;
        private decimal gratificacionMonto;
        private int gratificacionMesesTrabajados;
        private string gratificacionEstado;
        private string gratificacionObservaciones;
        public int GratificacionId { get => gratificacionId; set => gratificacionId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public string GratificacionSemestre { get => gratificacionSemestre; set => gratificacionSemestre = value; }
        public int GratificacionAnio { get => gratificacionAnio; set => gratificacionAnio = value; }
        public decimal GratificacionMonto { get => gratificacionMonto; set => gratificacionMonto = value; }
        public int GratificacionMesesTrabajados { get => gratificacionMesesTrabajados; set => gratificacionMesesTrabajados = value; }

        public string GratificacionEstado { get => gratificacionEstado; set => gratificacionEstado = value; }
        public string GratificacionObservaciones { get => gratificacionObservaciones; set => gratificacionObservaciones = value; }

        public bool EsPendiente()
        {
            return gratificacionEstado == "Pendiente";
        }

        public bool EstaPagada()
        {
            return gratificacionEstado == "Pagada";
        }
        
    }
}

