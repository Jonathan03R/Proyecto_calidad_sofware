using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class DetalleNomina
    {

        private int detalleNominaId;
        private Nomina nomina;
        private Trabajador trabajador;
        private Contrato contrato;

        private decimal sueldoBasico;
        private decimal remuneracionBruta;
        private decimal asignacionFamiliar;
        private decimal horasExtras;
        private decimal bonos;
        private decimal otrosIngresos;
        private decimal aporteEssalud;
        private decimal aporteONP;
        private decimal descuentoAFP;
        private decimal impuestoRentaMensual;
        private decimal descuentoTardanzas;
        private decimal descuentoFaltas;
        private decimal descuentoAdelantos;

        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;

        private bool tieneErrores;
        private string mensajeError;

        public int DetalleNominaId { get => detalleNominaId; set => detalleNominaId = value; }
        public Nomina Nomina { get => nomina; set => nomina = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }


        public Contrato Contrato { get => contrato; set => contrato = value; }
        public decimal SueldoBasico { get => sueldoBasico; set => sueldoBasico = value; }
        public decimal AsignacionFamiliar { get => asignacionFamiliar; set => asignacionFamiliar = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal RemuneracionBruta { get => remuneracionBruta; set => remuneracionBruta = value; }
        public decimal Bonos { get => bonos; set => bonos = value; }
        public decimal OtrosIngresos { get => otrosIngresos; set => otrosIngresos = value; }
        public decimal AporteEssalud { get => aporteEssalud; set => aporteEssalud = value; }
        public decimal AporteONP { get => aporteONP; set => aporteONP = value; }
        public decimal DescuentoAFP { get => descuentoAFP; set => descuentoAFP = value; }
        public decimal ImpuestoRentaMensual { get => impuestoRentaMensual; set => impuestoRentaMensual = value; }
        public decimal DescuentoTardanzas { get => descuentoTardanzas; set => descuentoTardanzas = value; }
        public decimal DescuentoFaltas { get => descuentoFaltas; set => descuentoFaltas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }
        public bool TieneErrores { get => tieneErrores; set => tieneErrores = value; }
        public string MensajeError { get => mensajeError; set => mensajeError = value; }

        public bool CalcularRemuneracionBruta()
        {
            // Validación de valores negativos
            if (sueldoBasico < 0 || asignacionFamiliar < 0 || horasExtras < 0 || bonos < 0)
            {
                tieneErrores = true;
                mensajeError = "Los valores de los ingresos no pueden ser negativos.";
                remuneracionBruta = 0;
                return false;
            }
            else
            {
                // Cálculo según la fórmula: RB = SB + AF + HE + BR
                remuneracionBruta = sueldoBasico + asignacionFamiliar + horasExtras + bonos;

                tieneErrores = false;
                mensajeError = string.Empty;
                return true;
            }

        }
        public bool CalcularAporteEssalud(bool redondear = true)
        {
            // Verificar que la remuneración bruta esté inicializada y no sea negativa
            if (remuneracionBruta < 0)
            {
                tieneErrores = true;
                mensajeError = "Remuneración bruta inválida para calcular aporte Essalud.";
                aporteEssalud = 0;
                return false;
            }

            // Cálculo: 9% de la remuneración bruta
            decimal aporte = remuneracionBruta * 0.09m;

            // Opcional: redondear a 2 decimales
            aporteEssalud = redondear ? Math.Round(aporte, 2) : aporte;

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularAporteONP(bool redondear = true)
        {
            // Verificar que la remuneración bruta esté inicializada y no sea negativa
            if (remuneracionBruta < 0)
            {
                tieneErrores = true;
                mensajeError = "Remuneración bruta inválida para calcular aporte ONP.";
                aporteONP = 0;
                return false;
            }

            // Cálculo: 13% de la remuneración bruta
            decimal aporte = remuneracionBruta * 0.13m;

            // Opcional: redondear a 2 decimales
            aporteONP = redondear ? Math.Round(aporte, 2) : aporte;

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularDescuentoAFP(decimal porComisionFlujo, decimal porComisionSaldo, decimal porSeguro, bool redondear = true)
        {
            if (remuneracionBruta < 0)
            {
                tieneErrores = true;
                mensajeError = "Remuneración bruta inválida para calcular descuento AFP.";
                descuentoAFP = 0;
                return false;
            }

            if (porComisionFlujo < 0 || porComisionSaldo < 0 || porSeguro < 0)
            {
                tieneErrores = true;
                mensajeError = "Porcentajes de comisión/seguro inválidos.";
                descuentoAFP = 0;
                return false;
            }

            decimal aporteFondo = remuneracionBruta * 0.10m;
            decimal aporteComision = 0m;
            decimal aporteSeguro = remuneracionBruta * porSeguro;

            // Decide si aplicar comisión por flujo o por saldo según cuál tenga valor en el TipoPension
            // (o según la propiedad TipoComisionAfpProp si la usas)
            if (porComisionFlujo > 0)
                aporteComision = remuneracionBruta * porComisionFlujo;
            else
                aporteComision = remuneracionBruta * porComisionSaldo;

            decimal totalDescuento = aporteFondo + aporteComision + aporteSeguro;
            descuentoAFP = redondear ? Math.Round(totalDescuento, 2) : totalDescuento;

            aporteONP = 0m; // limpiar ONP
            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }

        public bool CalcularPrevisionSegunTrabajador(bool redondear = true)
        {
            if (remuneracionBruta < 0)
            {
                tieneErrores = true;
                mensajeError = "Remuneración bruta inválida para cálculo previsional.";
                aporteONP = 0;
                descuentoAFP = 0;
                return false;
            }
            if (Trabajador == null || contrato.TipoPension == null)
            {
                tieneErrores = true;
                mensajeError = "Trabajador o TipoPension no asignado.";
                aporteONP = 0;
                descuentoAFP = 0;
                return false;
            }

            var tp = Contrato.TipoPension;
            if (string.Equals(tp.Entidad, "ONP", StringComparison.OrdinalIgnoreCase))
            {
                // ONP: 13%
                return CalcularAporteONP(redondear);
            }
            else if (string.Equals(tp.Entidad, "AFP", StringComparison.OrdinalIgnoreCase))
            {
                // AFP: usar porcentajes definidos en TipoPension
                return CalcularDescuentoAFP(tp.PorcentajeComisionFlujo, tp.PorcentajeComisionSaldo, tp.PorcentajeSeguro, redondear);
            }
            else
            {
                tieneErrores = true;
                mensajeError = "TipoPension.Entidad desconocida. Use 'ONP' o 'AFP'.";
                aporteONP = 0;
                descuentoAFP = 0;
                return false;
            }
        }

        public Gratificacion CalcularGratificacion(int mesesTrabajados, string semestre, int anio)
        {
            // Validar meses trabajados (de 0 a 6)
            if (mesesTrabajados < 0) mesesTrabajados = 0;
            if (mesesTrabajados > 6) mesesTrabajados = 6;

            // Fórmula: Gratificación = (Sueldo Básico + Asignación Familiar) × (Meses Trabajados / 6)
            decimal montoGratificacion = (SueldoBasico + AsignacionFamiliar) * (mesesTrabajados / 6.0m);

            // Crear y devolver el objeto Gratificación con los datos completos
            Gratificacion gratificacion = new Gratificacion
            {
                Trabajador = this.Trabajador,
                GratificacionSemestre = semestre,
                GratificacionAnio = anio,
                GratificacionMonto = montoGratificacion,
                GratificacionMesesTrabajados = mesesTrabajados,
                GratificacionEstado = "Calculado",
                GratificacionObservaciones = $"Gratificación correspondiente al semestre {semestre} del año {anio}"
            };

            return gratificacion;
        }


        public bool CalcularCTS(CTS cts)
        {
            if (cts == null || Trabajador == null)
            {
                tieneErrores = true;
                mensajeError = "No se puede calcular la CTS sin datos del trabajador o registro CTS.";
                return false;
            }

            // Validar rango de días trabajados (máximo 180 días por semestre)
            if (cts.CtsDiasTrabajados < 0) cts.CtsDiasTrabajados = 0;
            if (cts.CtsDiasTrabajados > 180) cts.CtsDiasTrabajados = 180;

            // Promedio de bonos (puedes ajustar si manejas varios meses)
            decimal promedioBonos = Bonos;

            // Fórmula: CTS = (Sueldo Básico + Asignación Familiar + Promedio de Bonos) × (Días Trabajados / 180)
            cts.CtsMonto = (SueldoBasico + AsignacionFamiliar + promedioBonos) * (cts.CtsDiasTrabajados / 180.0m);

            // Si todo está correcto
            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }

        public bool CalcularHorasExtras()
        {
            // Si las horas extras son negativas, se considera 0
            if (HorasExtras < 0)
            {
                tieneErrores = true;
                mensajeError = "Las horas extras no pueden ser negativas.";
                horasExtras = 0;
                return false;
            }

            // Si el sueldo básico es 0 o negativo, no se puede calcular
            if (SueldoBasico <= 0)
            {
                tieneErrores = true;
                mensajeError = "El sueldo básico debe ser mayor a cero para calcular las horas extras.";
                horasExtras = 0;
                return false;
            }

            // Jornada estándar: 30 días * 8 horas
            decimal valorHoraNormal = SueldoBasico / (30 * 8);
            decimal montoHorasExtras = 0;

            if (HorasExtras <= 2)
            {
                // Hasta 2 horas con recargo del 25%
                montoHorasExtras = HorasExtras * (valorHoraNormal * 1.25m);
            }
            else
            {
                // Primeras 2 horas con 25% y el resto con 35%
                decimal primerasDos = 2 * (valorHoraNormal * 1.25m);
                decimal adicionales = (HorasExtras - 2) * (valorHoraNormal * 1.35m);
                montoHorasExtras = primerasDos + adicionales;
            }

            // Se actualiza el campo horasExtras con el monto calculado
            horasExtras = Math.Round(montoHorasExtras, 2);

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularDescuentoTardanzas()
        {
            // Si las horas de tardanza son negativas, se considera 0
            if (DescuentoTardanzas < 0)
            {
                tieneErrores = true;
                mensajeError = "Las horas de tardanza no pueden ser negativas.";
                descuentoTardanzas = 0;
                return false;
            }

            // Validar sueldo básico
            if (SueldoBasico <= 0)
            {
                tieneErrores = true;
                mensajeError = "El sueldo básico debe ser mayor a cero para calcular el descuento por tardanzas.";
                descuentoTardanzas = 0;
                return false;
            }

            // Calcular el valor por hora normal (30 días × 8 horas diarias)
            decimal valorHora = SueldoBasico / (30 * 8);

            // Calcular el descuento total
            decimal descuento = DescuentoTardanzas * valorHora;

            // Asignar resultado
            descuentoTardanzas = Math.Round(descuento, 2);

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularDescuentoFaltas()
        {
            // Validar que el número de días de falta no sea negativo
            if (DescuentoFaltas < 0)
            {
                tieneErrores = true;
                mensajeError = "El número de días de falta no puede ser negativo.";
                descuentoFaltas = 0;
                return false;
            }

            // Validar que el sueldo básico sea válido
            if (SueldoBasico <= 0)
            {
                tieneErrores = true;
                mensajeError = "El sueldo básico debe ser mayor a cero para calcular el descuento por faltas.";
                descuentoFaltas = 0;
                return false;
            }

            // Suponiendo 30 días por mes
            decimal valorDia = SueldoBasico / 30;

            // Calcular el descuento total
            decimal descuento = DescuentoFaltas * valorDia;

            // Asignar el resultado redondeado a 2 decimales
            descuentoFaltas = Math.Round(descuento, 2);

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularDescuentoAdelantos()
        {
            // Validar que el monto no sea negativo
            if (DescuentoAdelantos < 0)
            {
                tieneErrores = true;
                mensajeError = "El monto de adelantos no puede ser negativo.";
                descuentoAdelantos = 0;
                return false;
            }

            // Si no existen adelantos, simplemente no hay descuento
            if (DescuentoAdelantos == 0)
            {
                tieneErrores = false;
                mensajeError = string.Empty;
                return true;
            }

            // El descuento es exactamente igual al monto de adelantos del periodo
            descuentoAdelantos = Math.Round(DescuentoAdelantos, 2);

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularAsignacionFamiliar(Parametro parametroRMV, bool tieneDerechoAsignacionFamiliar)
        {
            // Validar que el parámetro sea válido
            if (parametroRMV == null || parametroRMV.ParametroValor <= 0)
            {
                tieneErrores = true;
                mensajeError = "El valor de la Remuneración Mínima Vital no es válido.";
                asignacionFamiliar = 0;
                return false;
            }

            // Calcular asignación familiar según el derecho del trabajador
            if (tieneDerechoAsignacionFamiliar)
            {
                asignacionFamiliar = Math.Round(parametroRMV.ParametroValor * 0.10m, 2);
            }
            else
            {
                asignacionFamiliar = 0;
            }

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }
        public bool CalcularImpuestoRentaQuinta(List<ImpuestoRentaTramo> tramos, decimal valorUIT)
        {
            if (RemuneracionBruta <= 0 || tramos == null || tramos.Count == 0)
            {
                tieneErrores = true;
                mensajeError = "Datos insuficientes para calcular el impuesto a la renta.";
                impuestoRentaMensual = 0;
                return false;
            }

            // Paso 1: Calcular la remuneración bruta anual
            decimal remuneracionBrutaAnual = RemuneracionBruta * 12;

            // Paso 2: Calcular deducción anual de 7 UIT
            decimal deduccionAnual = 7 * valorUIT;

            // Paso 3: Calcular base imponible anual (BIA)
            decimal baseImponibleAnual = remuneracionBrutaAnual - deduccionAnual;
            if (baseImponibleAnual <= 0)
            {
                impuestoRentaMensual = 0;
                tieneErrores = false;
                mensajeError = "No aplica impuesto a la renta.";
                return true;
            }

            // Paso 4: Calcular impuesto anual según tramos progresivos
            decimal impuestoAnual = 0;
            decimal baseUIT = baseImponibleAnual / valorUIT;

            foreach (var tramo in tramos.OrderBy(t => t.NumeroTramo))
            {
                decimal limiteInferiorSoles = tramo.LimiteInferiorUIT * valorUIT;
                decimal limiteSuperiorSoles = tramo.LimiteSuperiorUIT * valorUIT;
                decimal tasa = tramo.TasaPorcentaje / 100;

                if (baseImponibleAnual > limiteSuperiorSoles)
                {
                    impuestoAnual += (limiteSuperiorSoles - limiteInferiorSoles) * tasa;
                }
                else if (baseImponibleAnual > limiteInferiorSoles)
                {
                    impuestoAnual += (baseImponibleAnual - limiteInferiorSoles) * tasa;
                    break;
                }
            }

            // Paso 5: Impuesto mensual (acumulativo anual dividido entre 12)
            impuestoRentaMensual = Math.Round(impuestoAnual / 12, 2);

            tieneErrores = false;
            mensajeError = string.Empty;
            return true;
        }

    }
}
