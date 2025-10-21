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
        private string nominaEstado;
        private int nominaTotalEmpleados;
        private decimal nominaTotalBruto;
        private decimal nominaTotalDescuentos;
        private decimal nominaTotalNeto;
        private string nominaObservaciones;

        private List<DetalleNomina> detalles;

        public int NominaId { get => nominaId; set => nominaId = value; }
        public Periodo Periodo { get => periodo; set => periodo = value; }
        public string NominaEstado { get => nominaEstado; set => nominaEstado = value; }
        public int NominaTotalEmpleados { get => nominaTotalEmpleados; set => nominaTotalEmpleados = value; }
        public decimal NominaTotalBruto { get => nominaTotalBruto; set => nominaTotalBruto = value; }
        public decimal NominaTotalDescuentos { get => nominaTotalDescuentos; set => nominaTotalDescuentos = value; }
        public decimal NominaTotalNeto { get => nominaTotalNeto; set => nominaTotalNeto = value; }
        public string NominaObservaciones { get => nominaObservaciones; set => nominaObservaciones = value; }
        public List<DetalleNomina> Detalles { get => detalles; set => detalles = value; }

        public void ActualizarTotales(decimal bruto, decimal descuentos, decimal neto)
        {
            nominaTotalBruto = bruto;
            nominaTotalDescuentos = descuentos;
            nominaTotalNeto = neto;
        }

        public bool PuedeCancelar()
        {
            return nominaEstado == "Procesando";
        }

        public void Cancelar(string motivo)
        {
            if (!PuedeCancelar())
                throw new InvalidOperationException("Solo se puede cancelar una nómina en estado Procesando.");
            nominaEstado = "Cancelado";
            nominaObservaciones = "ANULADO: " + motivo;
        }

        public bool ProcesarNomina(List<Trabajador> trabajadores)
        {
            if (trabajadores == null || trabajadores.Count == 0)
            {
                NominaObservaciones = "No hay trabajadores registrados para procesar.";
                return false;
            }

            // Filtrar solo los trabajadores activos
            var empleadosActivos = trabajadores.Where(t => t.EstaActivo()).ToList();

            if (empleadosActivos.Count == 0)
            {
                NominaObservaciones = "No existen trabajadores activos para procesar la nómina.";
                return false;
            }

            // Crear lista de detalles solo para trabajadores activos
            Detalles = new List<DetalleNomina>();

            foreach (var trabajador in empleadosActivos)
            {
                var detalle = new DetalleNomina
                {
                    Trabajador = trabajador
                    // Aquí luego se podrán invocar los cálculos (gratificación, CTS, descuentos, etc.)
                };

                Detalles.Add(detalle);
            }

            NominaTotalEmpleados = empleadosActivos.Count;
            NominaEstado = "Procesando";
            NominaObservaciones = "Nómina generada solo con trabajadores activos.";
            return true;
        }


    }
}
