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

        // RN-01 
        public void CalcularRemuneracionBruta()
        {
            remuneracionBruta = sueldoBasico + asignacionFamiliar + horasExtras + bonos;
        }

        public void CalcularAporteEssalud(bool redondear = true)
        {
            decimal aporte = remuneracionBruta * 0.09m;
            aporteEssalud = redondear ? Math.Round(aporte, 2) : aporte;
        }

        public void CalcularPrevisionSegunTrabajador(bool redondear = true)
        {
            var tp = Contrato.TipoPension;

            if (string.Equals(tp.Entidad, "ONP", StringComparison.OrdinalIgnoreCase))
            {
                CalcularAporteONP(redondear);
            }
            else if (string.Equals(tp.Entidad, "AFP", StringComparison.OrdinalIgnoreCase))
            {
                CalcularDescuentoAFP(tp.PorcentajeComisionFlujo, tp.PorcentajeComisionSaldo, tp.PorcentajeSeguro, redondear);
            }
        }
        public void CalcularAporteONP(bool redondear = true)
        {
            decimal aporte = remuneracionBruta * 0.13m;
            aporteONP = redondear ? Math.Round(aporte, 2) : aporte;
        }

        public void CalcularDescuentoAFP(decimal porComisionFlujo, decimal porComisionSaldo, decimal porSeguro, bool redondear = true)
        {
            decimal aporteFondo = remuneracionBruta * 0.10m;
            decimal aporteSeguro = remuneracionBruta * porSeguro;
            decimal totalDescuento;

            if (porComisionFlujo > 0)
            {
                decimal aporteComision = remuneracionBruta * porComisionFlujo;
                totalDescuento = aporteFondo + aporteComision + aporteSeguro;
            }
            else
            {
                totalDescuento = aporteFondo + aporteSeguro;
            }

            descuentoAFP = redondear ? Math.Round(totalDescuento, 2) : totalDescuento;
            aporteONP = 0m;
        }

        

        public void CalcularHorasExtras()
        {
            decimal valorHoraNormal = SueldoBasico / (30 * 8);
            decimal montoHorasExtras;

            if (HorasExtras <= 2)
            {
                montoHorasExtras = HorasExtras * valorHoraNormal * 1.25m;
            }
            else
            {
                montoHorasExtras = (2 * valorHoraNormal * 1.25m) +
                                  ((HorasExtras - 2) * valorHoraNormal * 1.35m);
            }

            horasExtras = Math.Round(montoHorasExtras, 2);
        }

        public void CalcularDescuentoTardanzas()
        {
            decimal valorHora = SueldoBasico / (30 * 8);
            descuentoTardanzas = Math.Round(DescuentoTardanzas * valorHora, 2);
        }

        public void CalcularDescuentoFaltas()
        {
            decimal valorDia = SueldoBasico / 30;
            descuentoFaltas = Math.Round(DescuentoFaltas * valorDia, 2);
        }

        public void CalcularDescuentoAdelantos()
        {
            descuentoAdelantos = Math.Round(DescuentoAdelantos, 2);
        }

        public void CalcularAsignacionFamiliar(Parametro parametroRMV, bool tieneDerechoAsignacionFamiliar)
        {
            asignacionFamiliar = tieneDerechoAsignacionFamiliar ?
                Math.Round(parametroRMV.ParametroValor * 0.10m, 2) : 0;
        }

        public void CalcularImpuestoRentaQuinta(List<ImpuestoRentaTramo> tramos, decimal valorUIT)
        {
            decimal remuneracionBrutaAnual = RemuneracionBruta * 12;
            decimal deduccionAnual = 7 * valorUIT;
            decimal baseImponibleAnual = remuneracionBrutaAnual - deduccionAnual;

            if (baseImponibleAnual <= 0)
            {
                impuestoRentaMensual = 0;
                return;
            }

            decimal baseImponibleUIT = baseImponibleAnual / valorUIT;
            decimal impuestoAnual = 0;

            if (baseImponibleUIT <= 5)
            {
                impuestoAnual = baseImponibleAnual * 0.08m;
            }
            else if (baseImponibleUIT <= 20)
            {
                decimal tramo1 = 5 * valorUIT * 0.08m;
                decimal tramo2 = (baseImponibleAnual - (5 * valorUIT)) * 0.14m;
                impuestoAnual = tramo1 + tramo2;
            }
            else if (baseImponibleUIT <= 35)
            {
                decimal tramo1 = 5 * valorUIT * 0.08m;
                decimal tramo2 = 15 * valorUIT * 0.14m;
                decimal tramo3 = (baseImponibleAnual - (20 * valorUIT)) * 0.17m;
                impuestoAnual = tramo1 + tramo2 + tramo3;
            }
            else if (baseImponibleUIT <= 45)
            {
                decimal tramo1 = 5 * valorUIT * 0.08m;
                decimal tramo2 = 15 * valorUIT * 0.14m;
                decimal tramo3 = 15 * valorUIT * 0.17m;
                decimal tramo4 = (baseImponibleAnual - (35 * valorUIT)) * 0.20m;
                impuestoAnual = tramo1 + tramo2 + tramo3 + tramo4;
            }
            else
            {
                decimal tramo1 = 5 * valorUIT * 0.08m;
                decimal tramo2 = 15 * valorUIT * 0.14m;
                decimal tramo3 = 15 * valorUIT * 0.17m;
                decimal tramo4 = 10 * valorUIT * 0.20m;
                decimal tramo5 = (baseImponibleAnual - (45 * valorUIT)) * 0.30m;
                impuestoAnual = tramo1 + tramo2 + tramo3 + tramo4 + tramo5;
            }

            impuestoRentaMensual = Math.Round(impuestoAnual / 12, 2);
        }

        public void CalcularTotales()
        {
            totalIngresos = remuneracionBruta + otrosIngresos;
            totalDescuentos = aporteEssalud + aporteONP + descuentoAFP + impuestoRentaMensual +
                             descuentoTardanzas + descuentoFaltas + descuentoAdelantos;
            netoPagar = totalIngresos - totalDescuentos;
        }
    }
}

    
