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


        public bool EstaProcesando() => nominaEstado == "Procesando";
        public bool EsExitosa() => nominaEstado == "Exitoso";
        public bool TieneErrores() => nominaEstado == "Con Errores";

        public void CalcularTotales()
        {
            if (detalles == null || detalles.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ No hay detalles para calcular totales.");
                return;
            }

            // Muestra cada total individual antes de sumar
            foreach (var d in detalles)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Empleado ID: {d.Contrato?.Trabajador?.TrabajadorId ?? 0} | " +
                    $"TotalIngresos: {d.TotalIngresos:F2} | " +
                    $"TotalDescuentos: {d.TotalDescuentos:F2} | " +
                    $"RemuneracionBruta: {d.RemuneracionBruta:F2}"
                );
            }

            // Suma con depuración
            nominaTotalEmpleados = detalles.Count;
            nominaTotalBruto = detalles.Sum(d => d.TotalIngresos);
            nominaTotalDescuentos = detalles.Sum(d => d.TotalDescuentos);
            nominaTotalNeto = detalles.Sum(d => d.RemuneracionBruta);

            System.Diagnostics.Debug.WriteLine($"📊 Total empleados: {nominaTotalEmpleados}");
            System.Diagnostics.Debug.WriteLine($"💰 Suma TotalIngresos: {nominaTotalBruto:F2}");
            System.Diagnostics.Debug.WriteLine($"💸 Suma TotalDescuentos: {nominaTotalDescuentos:F2}");
            System.Diagnostics.Debug.WriteLine($"🧾 Suma RemuneracionBruta: {nominaTotalNeto:F2}");
        }
        public static bool ExisteEnPeriodo(List<Nomina> nominas, int periodoId)
        {
            if (nominas == null || nominas.Count == 0)
                return false;

            return nominas.Any(n =>
                n.Periodo != null &&
                n.Periodo.PeriodoId == periodoId &&
                (n.nominaEstado == "Procesando" || n.nominaEstado == "Exitoso"));
        }
    }
}