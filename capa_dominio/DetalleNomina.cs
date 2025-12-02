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
        private decimal descuentoTardanzas;
        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;
        private decimal baseImponibleEsSalud; // ✅ Nueva propiedad

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
        public decimal DescuentoTardanzas { get => descuentoTardanzas; set => descuentoTardanzas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }
        public decimal BaseImponibleEsSalud { get => baseImponibleEsSalud; set => baseImponibleEsSalud = value; } // ✅ Nueva

        /// <summary>
        /// Bono por cumplimiento: S/ 50 si no tiene faltas ni tardanzas
        /// IMPORTANTE: Ejecutar DESPUÉS de calcular faltas y tardanzas
        /// </summary>
        public void CalculoBonosRegulares()
        {
            if (DescuentoFaltas == 0 && DescuentoTardanzas == 0)
                bonosRegulares = 50;
            else
                bonosRegulares = 0;
        }

        public void CalcularDescuentoTardanzas()
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular tardanzas.");

            if (HorasTrabajadas == null || HorasTrabajadas.Count == 0)
            {
                DescuentoTardanzas = 0;
                return;
            }

            decimal totalDescuento = 0;

            foreach (var r in HorasTrabajadas)
            {
                r.Contrato = Contrato;
                totalDescuento += r.CalcularDescuentoTardanza();
            }

            DescuentoTardanzas = Math.Round(totalDescuento, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// ✅ CORREGIDO: Descuento por faltas
        /// - Excluye SÁBADOS Y DOMINGOS (no solo domingos)
        /// - Solo días laborables cuentan como falta
        /// </summary>
        public void CalcularDescuentoFaltas()
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular faltas.");

            if (HorasTrabajadas == null)
            {
                System.Diagnostics.Trace.WriteLine("Las horas han llegado NULL");
                DescuentoFaltas = 0;
                return;
            }

            decimal sueldoPorDia = Contrato.ObtenerSueldoPorDia();

            // Agrupar por fecha de trabajo
            var diasTrabajados = HorasTrabajadas
                .GroupBy(h => h.Fecha.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            DateTime fechaInicio = nomina.Periodo.PeriodoFechaInicio;
            DateTime fechaFin = nomina.Periodo.PeriodoFechaFin;

            // ✅ CORREGIDO: Excluir SÁBADOS Y DOMINGOS
            var diasPeriodo = Enumerable
                .Range(0, (fechaFin.Date - fechaInicio.Date).Days + 1)
                .Select(offset => fechaInicio.Date.AddDays(offset))
                .Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday) // ✅ Excluir sábado y domingo
                .ToList();

            int totalFaltas = 0;

            foreach (var dia in diasPeriodo)
            {
                // Si no trabajó ese día → falta
                if (!diasTrabajados.ContainsKey(dia))
                {
                    totalFaltas++;
                }
                else
                {
                    var registros = diasTrabajados[dia];
                    // Si trabajó 0 horas normales, también cuenta como falta
                    if (registros.All(r => r.HorasNormales <= 0))
                        totalFaltas++;
                }
            }

            DescuentoFaltas = Math.Round(totalFaltas * sueldoPorDia, 2, MidpointRounding.AwayFromZero);

            System.Diagnostics.Trace.WriteLine(
                $"DESCUENTO FALTAS -> Faltas:{totalFaltas} | SueldoDia:{sueldoPorDia:F2} | TotalDescuento:{DescuentoFaltas:F2}"
            );
        }

        /// <summary>
        /// Cálculo de horas extras
        /// </summary>
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
                decimal pagoDiaExtras = h.CalcularPagoHorasExtras();

                if (pagoDiaExtras > 0)
                    totalExtras += pagoDiaExtras;
            }

            horasExtras = Math.Round(totalExtras, 2);
        }

        /// <summary>
        /// RN-01: Remuneración Bruta = Sueldo Básico + Asignación Familiar + Horas Extras + Bonos Regulares
        /// </summary>
        public void CalcularRemuneracionBruta()
        {
            remuneracionBruta = sueldoBasico + horasExtras + asignacionFamiliar + bonosRegulares;
        }

        /// <summary>
        /// RN-10: Asignación Familiar
        /// Monto fijo de S/ 113 (10% de RMV 2025) si tiene hijos
        /// </summary>
        public decimal CalculoAsignacionFamiliar(bool tieneHijos, decimal montoAsignacionFamiliar)
        {
            System.Diagnostics.Trace.WriteLine("CALCULANDO ASIGNACION FAMILIAR...");
            System.Diagnostics.Trace.WriteLine(
                $"ASIG_FAM -> Trabajador:{Contrato?.Trabajador?.TrabajadorId} | " +
                $"TieneHijos:{tieneHijos} | MontoFijo:{montoAsignacionFamiliar:F2}"
            );

            if (!tieneHijos)
            {
                asignacionFamiliar = 0;
                System.Diagnostics.Trace.WriteLine("ASIG_FAM -> No tiene hijos -> Monto: 0.00");
                return 0;
            }

            asignacionFamiliar = montoAsignacionFamiliar;

            System.Diagnostics.Trace.WriteLine(
                $"ASIG_FAM -> Tiene hijos -> Monto:{asignacionFamiliar:F2}"
            );

            return asignacionFamiliar;
        }

        /// <summary>
        /// Sistema de pensiones: ONP (13%) o AFP (10% + comisión)
        /// </summary>
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
                case 1: // ONP
                    // RN-03: Aporte ONP = Remuneración Bruta × 0.13
                    aporteONP = Math.Round(remuneracionBruta * 0.13m, 2, MidpointRounding.AwayFromZero);
                    break;

                case 2:
                case 3:
                case 4:
                case 5: // AFP
                    // RN-04: Descuento AFP = (RB × 0.10) + (RB × % Comisión Flujo)
                    var aporteObligatorio = remuneracionBruta * 0.10m;

                    var comision = 0m;
                    if (Contrato.TipoPension.ComisionSobreFlujo != null)
                        comision = remuneracionBruta * (decimal)Contrato.TipoPension.ComisionSobreFlujo;

                    descuentoAFP = Math.Round(aporteObligatorio + comision, 2, MidpointRounding.AwayFromZero);
                    break;

                case 6: // Sin sistema de pensión
                    break;

                default:
                    throw new InvalidOperationException($"Tipo de pensión con ID {tipoPensionId} no reconocido.");
            }
        }

        /// <summary>
        /// ✅ CORREGIDO: Aporte Essalud
        /// RN-02: 9% sobre la BASE IMPONIBLE (Sueldo Básico + Asignación Familiar)
        /// NO sobre la remuneración bruta total
        /// </summary>
        public void CalcularAporteEssalud(Parametro parametroEssalud)
        {
            if (parametroEssalud == null)
                throw new ArgumentNullException(nameof(parametroEssalud));

            decimal porcentaje = parametroEssalud.ParametroValor;

            // ✅ CORREGIDO: Base imponible = Sueldo Básico + Asignación Familiar
            baseImponibleEsSalud = sueldoBasico + asignacionFamiliar;
            decimal calculoBruto = baseImponibleEsSalud * porcentaje;

            System.Diagnostics.Trace.WriteLine(
                $"ESSALUD -> SueldoBasico: {sueldoBasico:F2} | AsigFam: {asignacionFamiliar:F2} | " +
                $"BaseImponible: {baseImponibleEsSalud:F2} | Porcentaje: {porcentaje:P2} | CalculoBruto: {calculoBruto:F2}"
            );

            aporteEssalud = Math.Round(calculoBruto, 2, MidpointRounding.AwayFromZero);

            System.Diagnostics.Trace.WriteLine(
                $"ESSALUD -> AporteEssalud (redondeado): {aporteEssalud:F2}"
            );
        }

        /// <summary>
        /// RN-05: Impuesto a la Renta de Quinta Categoría
        /// Cálculo acumulativo anual con tabla progresiva
        /// </summary>
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

            impuestoRentaMensual = Math.Round(impuestoAnual / 12, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Cálculo de totales finales
        /// Ingresos, descuentos y neto a pagar
        /// </summary>
        public void CalcularTotales()
        {
            // Ingresos = Remuneración bruta + otros ingresos
            totalIngresos = remuneracionBruta + otrosIngresos;

            // Total de descuentos
            totalDescuentos = aporteONP +
                              descuentoAFP +
                              impuestoRentaMensual +
                              descuentoFaltas +
                              descuentoTardanzas +
                              descuentoAdelantos +
                              aporteEssalud;

            netoPagar = totalIngresos - totalDescuentos;

            System.Diagnostics.Trace.WriteLine(
                $"TOTAL_INGRESOS: {totalIngresos} | TOTAL_DESCUENTOS: {totalDescuentos} | NETO_PAGAR: {netoPagar}"
            );
        }
    }
}