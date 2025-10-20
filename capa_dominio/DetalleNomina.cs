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

        // INGRESOS
        private decimal remuneracionBruta;
        private decimal sueldoBasico;
        private decimal asignacionFamiliar;
        private decimal horasExtras;
        private decimal bonosRegulares;
        private decimal otrosIngresos;

        // DESCUENTOS SEGÚN SISTEMA DE PENSIONES
        private string sistemaPensionAplicado; // 'ONP' o 'AFP'
        private decimal aporteEssalud;
        private decimal aporteONP;
        private decimal descuentoAFP;

        // IMPUESTO A LA RENTA
        private decimal remuneracionAcumuladaAnual;
        private decimal baseImponibleAnual;
        private decimal impuestoRentaAnual;
        private decimal impuestoRentaMensual;
        private decimal uitValor;
        private decimal deduccion7UIT;

        // OTROS DESCUENTOS
        private decimal descuentoTardanzas;
        private decimal descuentoFaltas;
        private decimal descuentoAdelantos;
        private decimal otrosDescuentos;

        // TOTALES
        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;

        // CONTROL
        private bool tieneErrores;
        private string mensajeError;

        // ======== Propiedades públicas ==========
        public int DetalleNominaId { get => detalleNominaId; set => detalleNominaId = value; }
        public Nomina Nomina { get => nomina; set => nomina = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }

        public decimal RemuneracionBruta { get => remuneracionBruta; set => remuneracionBruta = value; }
        public decimal SueldoBasico { get => sueldoBasico; set => sueldoBasico = value; }
        public decimal AsignacionFamiliar { get => asignacionFamiliar; set => asignacionFamiliar = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal BonosRegulares { get => bonosRegulares; set => bonosRegulares = value; }
        public decimal OtrosIngresos { get => otrosIngresos; set => otrosIngresos = value; }

        public string SistemaPensionAplicado { get => sistemaPensionAplicado; set => sistemaPensionAplicado = value; }
        public decimal AporteEssalud { get => aporteEssalud; set => aporteEssalud = value; }
        public decimal AporteONP { get => aporteONP; set => aporteONP = value; }
        public decimal DescuentoAFP { get => descuentoAFP; set => descuentoAFP = value; }

        public decimal RemuneracionAcumuladaAnual { get => remuneracionAcumuladaAnual; set => remuneracionAcumuladaAnual = value; }
        public decimal BaseImponibleAnual { get => baseImponibleAnual; set => baseImponibleAnual = value; }
        public decimal ImpuestoRentaAnual { get => impuestoRentaAnual; set => impuestoRentaAnual = value; }
        public decimal ImpuestoRentaMensual { get => impuestoRentaMensual; set => impuestoRentaMensual = value; }
        public decimal UITValor { get => uitValor; set => uitValor = value; }
        public decimal Deduccion7UIT { get => deduccion7UIT; set => deduccion7UIT = value; }

        public decimal DescuentoTardanzas { get => descuentoTardanzas; set => descuentoTardanzas = value; }
        public decimal DescuentoFaltas { get => descuentoFaltas; set => descuentoFaltas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal OtrosDescuentos { get => otrosDescuentos; set => otrosDescuentos = value; }

        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }

        public bool TieneErrores { get => tieneErrores; set => tieneErrores = value; }
        public string MensajeError { get => mensajeError; set => mensajeError = value; }

        // ======== Métodos de negocio ==========
        public void CalcularRemuneracionBruta()
        {
            remuneracionBruta = sueldoBasico + asignacionFamiliar + horasExtras + bonosRegulares + otrosIngresos;
        }

        public void CalcularAportes(decimal porcentajeEssalud, decimal porcentajeONP)
        {
            aporteEssalud = remuneracionBruta * porcentajeEssalud;
            if (sistemaPensionAplicado == "ONP")
                aporteONP = remuneracionBruta * porcentajeONP;
        }

        public void CalcularAFP(decimal porcentajeFondo, decimal comision, decimal seguro)
        {
            if (sistemaPensionAplicado == "AFP")
                descuentoAFP = remuneracionBruta * (porcentajeFondo + comision + seguro);
        }

        public void CalcularImpuestoRenta()
        {
            baseImponibleAnual = remuneracionAcumuladaAnual - deduccion7UIT;
            if (baseImponibleAnual <= 0)
            {
                impuestoRentaMensual = 0;
                return;
            }

            // Tabla progresiva simplificada
            if (baseImponibleAnual <= 5 * uitValor)
                impuestoRentaAnual = baseImponibleAnual * 0.08m;
            else if (baseImponibleAnual <= 20 * uitValor)
                impuestoRentaAnual = (5 * uitValor * 0.08m) + ((baseImponibleAnual - 5 * uitValor) * 0.14m);
            else if (baseImponibleAnual <= 35 * uitValor)
                impuestoRentaAnual = (5 * uitValor * 0.08m) + (15 * uitValor * 0.14m) + ((baseImponibleAnual - 20 * uitValor) * 0.17m);
            else
                impuestoRentaAnual = (5 * uitValor * 0.08m) + (15 * uitValor * 0.14m) + (15 * uitValor * 0.17m) + ((baseImponibleAnual - 35 * uitValor) * 0.20m);

            impuestoRentaMensual = impuestoRentaAnual / 12;
        }

        public void CalcularTotales()
        {
            totalIngresos = sueldoBasico + asignacionFamiliar + horasExtras + bonosRegulares + otrosIngresos;
            totalDescuentos = aporteONP + descuentoAFP + descuentoTardanzas + descuentoFaltas + descuentoAdelantos + otrosDescuentos + impuestoRentaMensual;
            netoPagar = totalIngresos - totalDescuentos;
        }

        public decimal ObtenerTotalIngresos()
        {
            return totalIngresos;
        }

        public decimal ObtenerTotalDescuentos()
        {
            return totalDescuentos;
        }

        public decimal ObtenerNetoPagar()
        {
            return netoPagar;
        }

        public void MarcarError(string mensaje)
        {
            tieneErrores = true;
            mensajeError = mensaje;
        }
    }
}
