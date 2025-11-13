using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio
{
    public class DetalleNomina
    {
        private int detalleNominaId;
        private Nomina nomina;
        private Contrato contrato;
        private DetalleParametro detalleParametro;
        private AdelantoSueldo adelantoSueldo;
        private List<HoraTrabajada> horasTrabajadas;
        private List<TipoHoraExtra> tiposHorasExtras;

        private decimal sueldoBasico;
        private decimal remuneracionBruta;
        private decimal asignacionFamiliar;
        private decimal horasExtras;
        private decimal bonosRegulares;
        private decimal otrosIngresos;
        private decimal aporteEssalud;
        private decimal aporteONP;
        private decimal descuentoAFP;
        private decimal impuestoRentaMensual;
        private string sistemasPensionAplicado;
        private decimal descuentoFaltas;
        private decimal descuentoAdelantos;

        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;

        public int DetalleNominaId { get => detalleNominaId; set => detalleNominaId = value; }
        public Nomina Nomina { get => nomina; set => nomina = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public DetalleParametro DetalleParametro { get => detalleParametro; set => detalleParametro = value; }
        public AdelantoSueldo AdelantoSueldo { get => adelantoSueldo; set => adelantoSueldo = value; }
        public List<HoraTrabajada> HorasTrabajadas { get => horasTrabajadas; set => horasTrabajadas = value; }
        public List<TipoHoraExtra> TiposHorasExtras { get => tiposHorasExtras; set => tiposHorasExtras = value; }

        public decimal SueldoBasico { get => sueldoBasico; set => sueldoBasico = value; }
        public decimal AsignacionFamiliar { get => asignacionFamiliar; set => asignacionFamiliar = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal RemuneracionBruta { get => remuneracionBruta; set => remuneracionBruta = value; }
        public decimal BonosRegulares { get => bonosRegulares; set => bonosRegulares = value; }
        public decimal OtrosIngresos { get => otrosIngresos; set => otrosIngresos = value; }
        public decimal AporteEssalud { get => aporteEssalud; set => aporteEssalud = value; }
        public decimal AporteONP { get => aporteONP; set => aporteONP = value; }
        public decimal DescuentoAFP { get => descuentoAFP; set => descuentoAFP = value; }
        public decimal ImpuestoRentaMensual { get => impuestoRentaMensual; set => impuestoRentaMensual = value; }
        public string SistemasPensionAplicado { get => sistemasPensionAplicado; set => sistemasPensionAplicado = value; }
        public decimal DescuentoFaltas { get => descuentoFaltas; set => descuentoFaltas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }

        // =========================
        // SUELDO SEGÚN ASISTENCIA
        // =========================

        public void CalcularSueldoSegunAsistencia(DateTime fechaInicio, DateTime fechaFin)
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular sueldo.");

            if (!Contrato.ContratoHorasSemanales.HasValue || Contrato.ContratoHorasSemanales.Value <= 0)
                throw new InvalidOperationException("El contrato no tiene configuradas las horas semanales.");

            if (HorasTrabajadas == null)
                HorasTrabajadas = new List<HoraTrabajada>();

            decimal jornadaDiariaHoras = Contrato.ContratoHorasSemanales.Value / 6m;
            decimal sueldoPorDia = Math.Round(Contrato.ContratoSalario / 30m, 2);

            var diasPeriodo = Enumerable
                .Range(0, (fechaFin.Date - fechaInicio.Date).Days + 1)
                .Select(offset => fechaInicio.Date.AddDays(offset))
                .ToList();

            var horasPorDia = HorasTrabajadas
                .GroupBy(h => h.Fecha.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            int diasPagados = 0;
            int diasFalta = 0;

            foreach (var dia in diasPeriodo)
            {
                if (dia.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (!horasPorDia.TryGetValue(dia, out var registrosDelDia) || registrosDelDia.Count == 0)
                {
                    diasFalta++;
                    continue;
                }

                var registro = registrosDelDia[0];
                registro.Contrato = Contrato;

                if (registro.EsFalta())
                {
                    diasFalta++;
                    continue;
                }

                if (registro.TieneTardanza(jornadaDiariaHoras))
                {
                    diasFalta++;
                    continue;
                }

                diasPagados++;
            }

            sueldoBasico = Math.Round(diasPagados * sueldoPorDia, 2);
            descuentoFaltas = Math.Round(diasFalta * sueldoPorDia, 2);

            System.Diagnostics.Trace.WriteLine(
                $"ASISTENCIA -> DiasPagados: {diasPagados} | DiasFalta: {diasFalta} | SueldoBasico: {sueldoBasico} | DescuentoFaltas: {descuentoFaltas}"
            );
        }

        // =========================
        // HORAS EXTRAS
        // =========================

        public void CalcularPagoTotalHorasExtras()
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo en el detalle de nómina.");

            if (HorasTrabajadas == null || HorasTrabajadas.Count == 0)
            {
                horasExtras = 0;
                return;
            }

            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
                throw new InvalidOperationException("No se cargaron los tipos de horas extras.");

            decimal totalExtras = 0m;

            foreach (var h in HorasTrabajadas)
            {
                h.Contrato = Contrato;
                h.TiposHorasExtras = TiposHorasExtras;

                decimal pagoDia = h.CalcularPagoDia();
                decimal pagoNormalDia = h.HorasNormales * Contrato.ContratoTarifaHora;
                decimal extraDia = pagoDia - pagoNormalDia;

                if (extraDia > 0)
                    totalExtras += extraDia;
            }

            horasExtras = Math.Round(totalExtras, 2);
        }

        // =========================
        // ASIGNACIÓN FAMILIAR
        // =========================

        public decimal CalculoAsignacionFamiliar(bool tieneRemuneracionFamiliar)
        {
            if (!tieneRemuneracionFamiliar || Contrato == null)
            {
                asignacionFamiliar = 0;
                return 0;
            }

            asignacionFamiliar = Math.Round(Contrato.ContratoSalario * 0.10m, 2);
            return asignacionFamiliar;
        }

        // =========================
        // REMUNERACIÓN BRUTA
        // =========================

        public void CalcularRemuneracionBruta()
        {
            remuneracionBruta =
                sueldoBasico +
                asignacionFamiliar +
                horasExtras +
                bonosRegulares +
                otrosIngresos;
        }

        // =========================
        // SISTEMA DE PENSIONES
        // =========================

        public void CalcularSistemaPensiones()
        {
            if (Contrato == null || Contrato.TipoPension == null)
                throw new InvalidOperationException("El contrato o el tipo de pensión no están definidos.");

            int tipoPensionId = Contrato.TipoPension.TipoPensionId;

            aporteONP = 0;
            descuentoAFP = 0;
            sistemasPensionAplicado = Contrato.TipoPension.Nombre;

            switch (tipoPensionId)
            {
                case 1:
                    aporteONP = Math.Round(remuneracionBruta * 0.13m, 2);
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                    descuentoAFP = Math.Round(remuneracionBruta * 0.10m, 2);
                    break;

                case 6:
                    break;

                default:
                    throw new InvalidOperationException($"Tipo de pensión con ID {tipoPensionId} no reconocido.");
            }
        }

        // =========================
        // ESSALUD
        // =========================

        public void CalcularAporteEssalud(Parametro parametroEssalud)
        {
            if (parametroEssalud == null)
                throw new ArgumentNullException(nameof(parametroEssalud));

            aporteEssalud = Math.Round(remuneracionBruta * parametroEssalud.ParametroValor, 2);
        }

        // =========================
        // RENTA DE QUINTA
        // =========================

        public void CalcularImpuestoRentaQuinta(List<ImpuestoRentaTramo> tramos, decimal valorUIT)
        {
            if (tramos == null || tramos.Count == 0)
            {
                impuestoRentaMensual = 0;
                return;
            }

            decimal remuneracionBrutaAnual = remuneracionBruta * 12;
            decimal deduccionAnual = 7 * valorUIT;
            decimal baseImponibleAnual = remuneracionBrutaAnual - deduccionAnual;

            if (baseImponibleAnual <= 0)
            {
                impuestoRentaMensual = 0;
                return;
            }

            decimal baseImponibleUIT = baseImponibleAnual / valorUIT;
            decimal impuestoAnual = 0m;

            foreach (var tramo in tramos.OrderBy(t => t.NumeroTramo))
            {
                decimal limiteInferior = tramo.LimiteInferiorUIT;
                decimal limiteSuperior = tramo.LimiteSuperiorUIT ?? baseImponibleUIT;

                if (limiteSuperior == 0)
                    limiteSuperior = baseImponibleUIT;

                decimal rangoTramo = Math.Min(baseImponibleUIT, limiteSuperior) - limiteInferior;

                if (rangoTramo > 0)
                {
                    decimal montoTramo = rangoTramo * valorUIT;
                    decimal tasa = tramo.TasaPorcentaje / 100m;
                    impuestoAnual += montoTramo * tasa;
                }

                if (baseImponibleUIT <= limiteSuperior)
                    break;
            }

            impuestoRentaMensual = Math.Round(impuestoAnual / 12, 2);
        }

        // =========================
        // TOTALES
        // =========================

        public void CalcularTotales()
        {
            totalIngresos = remuneracionBruta;

            totalDescuentos =
                aporteONP +
                descuentoAFP +
                impuestoRentaMensual +
                descuentoFaltas +
                descuentoAdelantos;

            netoPagar = totalIngresos - totalDescuentos;

            System.Diagnostics.Trace.WriteLine(
                $"TOTAL_INGRESOS: {totalIngresos} | TOTAL_DESCUENTOS: {totalDescuentos} | NETO_PAGAR: {netoPagar}"
            );
        }
    }
}
