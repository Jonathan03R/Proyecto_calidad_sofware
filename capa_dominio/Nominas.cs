using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Nomina
    {
        // Estados de la nómina (evitar magic strings)
        public const string EstadoProcesando = "Procesando";
        public const string EstadoExitosa = "Exitoso";
        public const string EstadoConErrores = "Con Errores";

        // Auto-propiedades
        public int NominaId { get; set; }
        public Periodo Periodo { get; set; }
        public DateTime NominaFecha { get; set; }
        public DateTime NominaFechaProcesamiento { get; set; }
        public string NominaEstado { get; set; }
        public int NominaTotalEmpleados { get; private set; }
        public decimal NominaTotalBruto { get; private set; }
        public decimal NominaTotalDescuentos { get; private set; }
        public decimal NominaTotalNeto { get; private set; }
        public string NominaObservaciones { get; set; }

        public List<DetalleNomina> Detalles { get; set; } = new List<DetalleNomina>();

        // Helpers de estado
        public bool EstaProcesando() => NominaEstado == EstadoProcesando;
        public bool EsExitosa() => NominaEstado == EstadoExitosa;
        public bool TieneErrores() => NominaEstado == EstadoConErrores;

        public void CalcularTotales()
        {
            if (Detalles == null || Detalles.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ No hay detalles para calcular totales.");
                return;
            }

            foreach (var det in Detalles)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Empleado ID: {det.Contrato?.Trabajador?.TrabajadorId ?? 0} | " +
                    $"Bruto: {det.RemuneracionBruta:F2} | " +
                    $"Ingresos: {det.TotalIngresos:F2} | " +
                    $"Descuentos: {det.TotalDescuentos:F2} | " +
                    $"Neto: {det.NetoPagar:F2}"
                );
            }

            NominaTotalEmpleados = Detalles.Count;
            NominaTotalBruto = Detalles.Sum(det => det.RemuneracionBruta);
            NominaTotalDescuentos = Detalles.Sum(det => det.TotalDescuentos);
            NominaTotalNeto = Detalles.Sum(det => det.NetoPagar);

            System.Diagnostics.Debug.WriteLine($"📊 Total empleados: {NominaTotalEmpleados}");
            System.Diagnostics.Debug.WriteLine($"💰 Total Bruto: {NominaTotalBruto:F2}");
            System.Diagnostics.Debug.WriteLine($"💸 Total Descuentos: {NominaTotalDescuentos:F2}");
            System.Diagnostics.Debug.WriteLine($"🧾 Total Neto: {NominaTotalNeto:F2}");
        }

        public static bool ExisteEnPeriodo(IReadOnlyCollection<Nomina> nominas, int periodoId)
        {
            if (nominas == null || nominas.Count == 0)
            {
                return false;
            }

            return nominas.Any(n =>
                n.Periodo != null &&
                n.Periodo.PeriodoId == periodoId &&
                (n.NominaEstado == EstadoProcesando || n.NominaEstado == EstadoExitosa));
        }
    }
}