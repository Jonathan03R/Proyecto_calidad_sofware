using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace capa_dominio
{
    public class DetalleNomina
    {
        public int DetalleNominaId { get; set; }
        public Nomina Nomina { get; set; }
        public Contrato Contrato { get; set; }
        public DetalleParametro DetalleParametro { get; set; }
        public AdelantoSueldo AdelantoSueldo { get; set; }
        public List<HoraTrabajada> HorasTrabajadas { get; set; }
        public List<TipoHoraExtra> TiposHorasExtras { get; set; }

        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal RemuneracionBruta { get; set; }
        public decimal BonosRegulares { get; set; }
        public decimal OtrosIngresos { get; set; }
        public decimal AporteEssalud { get; set; }
        public decimal AporteONP { get; set; }
        public decimal DescuentoAFP { get; set; }
        public decimal ImpuestoRentaMensual { get; set; }
        public string SistemasPensionAplicado { get; set; }
        public decimal DescuentoFaltas { get; set; }
        public decimal DescuentoTardanzas { get; set; }
        public decimal DescuentoAdelantos { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal NetoPagar { get; set; }



        /// <summary>
        /// regla: si el trabajador tiene 0 faltas y 0 tardanzas en el periodo,
        /// recibe un bono fijo de 50 soles por cumplimiento.
        /// </summary>
        public void CalculoBonosRegulares()
        {
            if (DescuentoFaltas == 0 && DescuentoTardanzas == 0)
                BonosRegulares = 50;
            else
                BonosRegulares = 0;
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
        /// regla de negocio para faltas:
        /// se considera falta cada día laboral del periodo en el que el trabajador:
        /// 1) no tiene ningún registro de asistencia
        /// 2) o tiene registros pero con 0 horas normales trabajadas
        /// los domingos no cuentan como día laboral
        /// el descuento final es (total_faltas * sueldo_por_dia)
        /// </summary>
       
        public void CalcularDescuentoFaltas()
        {
            if (Contrato == null)
                throw new InvalidOperationException("El contrato no puede ser nulo para calcular faltas.");

            if (HorasTrabajadas == null)
            {
                System.Diagnostics.Trace.WriteLine(
                $"las horas han llegado NULL"
            );
                DescuentoFaltas = 0;
                return;
            }

            decimal sueldoPorDia = Contrato.ObtenerSueldoPorDia();

            var diasTrabajados = HorasTrabajadas
                .GroupBy(h => h.Fecha.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            DateTime fechaInicio = Nomina.Periodo.PeriodoFechaInicio;
            DateTime fechaFin = Nomina.Periodo.PeriodoFechaFin;

            var diasPeriodo = Enumerable
                .Range(0, (fechaFin.Date - fechaInicio.Date).Days + 1)
                .Select(offset => fechaInicio.Date.AddDays(offset))
                .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                .ToList();

            int totalFaltas = 0;

            foreach (var dia in diasPeriodo)
            {
                if (!diasTrabajados.ContainsKey(dia))
                {
                    totalFaltas++;
                }
                else
                {
                    var registros = diasTrabajados[dia];
                    if (registros.All(r => r.HorasNormales <= 0))
                        totalFaltas++;
                }
            }

            DescuentoFaltas = totalFaltas * sueldoPorDia;

            System.Diagnostics.Trace.WriteLine(
                $"DESCUENTO FALTAS -> Faltas:{totalFaltas} | SueldoDia:{sueldoPorDia:F2} | TotalDescuento:{DescuentoFaltas:F2}"
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
                HorasExtras = 0;
                return;
            }

            if (TiposHorasExtras == null || TiposHorasExtras.Count == 0)
            {
                throw new InvalidOperationException("No se cargaron los tipos de horas extras.");
            }
            
            decimal totalExtras = 0m;

            foreach (var h in HorasTrabajadas)
            {
                h.Contrato = Contrato;
                h.TiposHorasExtras = TiposHorasExtras;

                decimal pagoDiaExtras = h.CalcularPagoHorasExtras();

                if (pagoDiaExtras > 0)
                    totalExtras += pagoDiaExtras;
            }

            HorasExtras = totalExtras;
        }


        public void CalcularRemuneracionBruta()
        {
            RemuneracionBruta = Contrato.ContratoSalario + HorasExtras + AsignacionFamiliar + BonosRegulares;
        }


        // =========================
        // ASIGNACIÓN FAMILIAR
        // =========================

        public decimal CalculoAsignacionFamiliar(bool tieneRemuneracionFamiliar)
        {
            System.Diagnostics.Trace.WriteLine("CALCULANDO ASIGNACION FAMILIAR...");
            System.Diagnostics.Trace.WriteLine(
                $"ASIG_FAM -> Trabajador:{Contrato?.Trabajador?.TrabajadorId} | " +
                $"TieneFam:{tieneRemuneracionFamiliar} | Salario:{Contrato?.ContratoSalario:F2}"
            );

            if (!tieneRemuneracionFamiliar)
            {
                AsignacionFamiliar = 0;
                return 0;
            }

            AsignacionFamiliar = 113m;
            return AsignacionFamiliar;
        }


        // =========================
        // SISTEMA DE PENSIONES
        // =========================

        public void CalcularSistemaPensiones()
        {

            if (Contrato == null || Contrato.TipoPension == null)
                throw new InvalidOperationException("El contrato o el tipo de pensión no están definidos.");

            int tipoPensionId = Contrato.TipoPension.TipoPensionId;

            AporteONP = 0;
            DescuentoAFP = 0;
            SistemasPensionAplicado = Contrato.TipoPension.Nombre;

            switch (tipoPensionId)
            {
                case 1:
                    AporteONP = Math.Round(RemuneracionBruta * 0.13m, 2, MidpointRounding.AwayFromZero);
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                    var aporteObligatorio = RemuneracionBruta * 0.10m;

                    var comision = 0m;
                    Debug.WriteLine("=== DEBUG COMISION SOBRE FLUJO ===");
                    Debug.WriteLine($"remuneracion_bruta: {RemuneracionBruta}");
                    Debug.WriteLine($"comision_sobre_flujo_raw: {Contrato.TipoPension.ComisionSobreFlujo}");

                    if (Contrato.TipoPension.ComisionSobreFlujo != null)
                    {
                        comision = RemuneracionBruta * (decimal)Contrato.TipoPension.ComisionSobreFlujo;
                        Debug.WriteLine($"comision_calculada: {comision}");
                    }
                    else
                    {
                        Debug.WriteLine("comision_sobre_flujo_es_null");
                    }

                    DescuentoAFP = Math.Round(aporteObligatorio + comision, 2, MidpointRounding.AwayFromZero);
                    Debug.WriteLine($"descuento_afp_total: {DescuentoAFP}");
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
            {
                throw new ArgumentNullException(nameof(parametroEssalud));
            }
            decimal porcentaje = parametroEssalud.ParametroValor;
            decimal calculoBruto = RemuneracionBruta * porcentaje;

            Trace.TraceInformation(
                $"ESSALUD -> RemuneracionBruta: {RemuneracionBruta:F2} | Porcentaje: {porcentaje:P2} | CalculoBruto: {calculoBruto:F2}"
            );
            AporteEssalud = calculoBruto;

            Trace.TraceInformation(
                $"ESSALUD -> AporteEssalud (redondeado): {AporteEssalud:F2}"
            );

        }

        // =========================
        // RENTA DE QUINTA
        // =========================

        public void CalcularImpuestoRentaQuinta(List<ImpuestoRentaTramo> tramos, decimal valorUIT)
        {
            if (tramos == null || tramos.Count == 0)
            {
                ImpuestoRentaMensual = 0;
                return;
            }

            decimal remuneracionBrutaAnual = RemuneracionBruta * 12m;
            decimal deduccionAnual = 7m * valorUIT;
            decimal baseImponibleAnual = remuneracionBrutaAnual - deduccionAnual;

            if (baseImponibleAnual <= 0)
            {
                ImpuestoRentaMensual = 0;
                return;
            }

            decimal baseImponibleEnUIT = baseImponibleAnual / valorUIT;
            decimal impuestoCalculado = 0m;

            foreach (var tramo in tramos.OrderBy(t => t.NumeroTramo))
            {
                decimal limiteInferiorUIT = tramo.LimiteInferiorUIT;
                decimal limiteSuperiorUIT = tramo.LimiteSuperiorUIT ?? decimal.MaxValue;

                if (baseImponibleEnUIT <= limiteInferiorUIT)
                    break;

                decimal rangoAplicableUIT = Math.Min(baseImponibleEnUIT, limiteSuperiorUIT) - limiteInferiorUIT;

                if (rangoAplicableUIT > 0)
                {
                    decimal montoRangoSoles = rangoAplicableUIT * valorUIT;
                    decimal tasaDecimal = tramo.TasaPorcentaje / 100m;

                    impuestoCalculado += montoRangoSoles * tasaDecimal;
                }
            }

            ImpuestoRentaMensual = impuestoCalculado / 12m;
        }


        // =========================
        // TOTALES
        // =========================

        public void CalcularTotales()
        {
            TotalIngresos = RemuneracionBruta + OtrosIngresos;

            TotalDescuentos = AporteONP +
                              DescuentoAFP +
                              ImpuestoRentaMensual +
                              DescuentoFaltas +
                              DescuentoAdelantos + DescuentoTardanzas;

            NetoPagar = TotalIngresos - TotalDescuentos;

           
        }

    }
}