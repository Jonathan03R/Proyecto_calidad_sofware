using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Nomina
    {
        
        private int nominaId;
        private Periodo periodo; 
        private DateTime nominaFecha;
        private DateTime nominaFechaProcesamiento;
        private string nominaEstado;
        private int nominaTotalEmpleados;
        private decimal nominaTotalBruto;
        private decimal nominaTotalDescuentos;
        private decimal nominaTotalNeto;
        private string nominaObservaciones;
        private List<DetalleNomina> detalles; 
        public int NominaId { get => nominaId; set => nominaId = value; }
        public Periodo Periodo { get => periodo; set => periodo = value; }
        public DateTime NominaFecha { get => nominaFecha; set => nominaFecha = value; }
        public DateTime NominaFechaProcesamiento { get => nominaFechaProcesamiento; set => nominaFechaProcesamiento = value; }
        public string NominaEstado { get => nominaEstado; set => nominaEstado = value; }
        public int NominaTotalEmpleados { get => nominaTotalEmpleados; set => nominaTotalEmpleados = value; }
        public decimal NominaTotalBruto { get => nominaTotalBruto; set => nominaTotalBruto = value; }
        public decimal NominaTotalDescuentos { get => nominaTotalDescuentos; set => nominaTotalDescuentos = value; }
        public decimal NominaTotalNeto { get => nominaTotalNeto; set => nominaTotalNeto = value; }
        public string NominaObservaciones { get => nominaObservaciones; set => nominaObservaciones = value; }
        public List<DetalleNomina> Detalles { get => detalles; set => detalles = value; }

        
        public bool EstaProcesando()
        {
            return nominaEstado == "Procesando";
        }

        public bool EsExitosa()
        {
            return nominaEstado == "Exitoso";
        }

        public bool TieneErrores()
        {
            return nominaEstado == "Con Errores";
        }

        public void CalcularTotales()
        {
            if (detalles == null || detalles.Count == 0)
                return;

            nominaTotalEmpleados = detalles.Count;
            nominaTotalBruto = detalles.Sum(d => d.TotalIngresos);
            nominaTotalDescuentos = detalles.Sum(d => d.TotalDescuentos);
            nominaTotalNeto = detalles.Sum(d => d.RemuneracionBruta);
        }

        public void MarcarComoCancelada(string observacion)
        {
            nominaEstado = "Cancelado";
            nominaObservaciones = observacion;
        }
    }
}