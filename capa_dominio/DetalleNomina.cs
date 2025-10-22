using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class DetalleNomina
    {

        private int detalleNominaId;
        private Nomina nomina;
        private Contrato contrato;
        private DetalleParametro detalleParametro;
        private Tardanza tardanza;
        private AdelantoSueldo adelantoSueldo;
        private Falta falta;
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
        //private decimal descuentoTardanzas;
        private decimal descuentoFaltas;
        private decimal descuentoAdelantos;

        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;

        private bool tieneErrores;
        private string mensajeError;

        public int DetalleNominaId { get => detalleNominaId; set => detalleNominaId = value; }
        public Nomina Nomina { get => nomina; set => nomina = value; }
       


        public Contrato Contrato { get => contrato; set => contrato = value; }
        public Falta Falta { get => falta; set => falta = value; }
        public AdelantoSueldo AdelantoSueldo { get => adelantoSueldo; set => adelantoSueldo = value; }
        public Tardanza Tardanzas { get => tardanza; set => tardanza = value; }
        public DetalleParametro DetalleParametro { get => detalleParametro; set => detalleParametro = value; }
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
        //public decimal DescuentoTardanzas { get => descuentoTardanzas; set => descuentoTardanzas = value; }
        public decimal DescuentoFaltas { get => descuentoFaltas; set => descuentoFaltas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }
        public bool TieneErrores { get => tieneErrores; set => tieneErrores = value; }
        public string MensajeError { get => mensajeError; set => mensajeError = value; }

        // La remuneración bruta incluye: RB = Sueldo Básico + Asignación Familiar + Horas Extras + Bonos Regulares
        public void CalcularRemuneracionBruta()
        {
            
            remuneracionBruta = contrato.ContratoSalario + asignacionFamiliar + horasExtras + bonosRegulares;
        }


        //La asignación familiar es de 10% de la Remuneración Mínima Legal vigente.
        //Asignación Familiar = Remuneración Mínima Vital × 0.10 si el trabajador tiene derecho; en caso contrario, 0.

        public decimal CalculoAsignacionFamiliar(bool tieneRemuneacionFamiliar)
        {
            if (tieneRemuneacionFamiliar) { 
                return asignacionFamiliar = contrato.ContratoSalario * 0.1m;
            }
            return 0;
        }


//        Las horas extras tienen recargos del 25% para las primeras 2 horas y 35% para horas adicionales, sobre el valor hora del sueldo básico.
//Valor Hora Extra = Valor Hora Normal × (1 + 0.25) para primeras 2 horas
//Valor Hora Extra = Valor Hora Normal × (1 + 0.35) para horas adicionales

        public void CalcularHorasExtras()
        {
            decimal valorHoraNormal = contrato.ContratoSalario / (30 * 8);
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


            //El aporte a AFP comprende: 10% para fondo de pensiones, comisión administrativa variable y seguro de invalidez(SIS).
     //       El aporte a ONP corresponde al 13% de la remuneración bruta.
     //Aporte ONP = Remuneración Bruta × 0.13.


        public void CalcularSistemaPensiones()
        {
            var tp = Contrato.TipoPension;

            if (string.Equals(tp.Entidad, "ONP", StringComparison.OrdinalIgnoreCase))
            {
                aporteONP = remuneracionBruta * 0.13m;
            }
            else
            {
                descuentoAFP = remuneracionBruta * 0.10m;
            }
        }

        

        //public void CalcularDescuentoFaltas()
        //{
        //    decimal valorDia = SueldoBasico / 30;
        //    descuentoFaltas = Math.Round(DescuentoFaltas * valorDia, 2);
        //}

        //public void CalcularDescuentoAdelantos()
        //{
        //    descuentoAdelantos = Math.Round(DescuentoAdelantos, 2);
        //}

        public void CalcularAsignacionFamiliar(Parametro parametroRMV, bool tieneDerechoAsignacionFamiliar)
        {
            asignacionFamiliar = tieneDerechoAsignacionFamiliar ?
                Math.Round(parametroRMV.ParametroValor * 0.10m, 2) : 0;
        }
//        El aporte a Essalud corresponde al 9% de la remuneración bruta y es costo del empleador.
//Aporte Essalud = Remuneración Bruta × 0.09

        public void CalcularAporteEssalud(Parametro parametroEssalud)
        {
         
            aporteEssalud = Math.Round(remuneracionBruta * parametroEssalud.ParametroValor, 2);
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

            // Convertir la base imponible a UIT
            decimal baseImponibleUIT = baseImponibleAnual / valorUIT;
            decimal impuestoAnual = 0m;
            decimal acumuladoUIT = 0m;

            foreach (var tramo in tramos.OrderBy(t => t.NumeroTramo))
            {
                // Determinar los límites del tramo
                decimal limiteInferior = tramo.LimiteInferiorUIT;
                decimal limiteSuperior = tramo.LimiteSuperiorUIT;

                // Si el tramo superior es 0 o muy alto, considerarlo sin límite
                if (limiteSuperior == 0)
                    limiteSuperior = baseImponibleUIT;

                // Calcular cuánto de la base cae en este tramo
                decimal rangoTramo = Math.Min(baseImponibleUIT, limiteSuperior) - limiteInferior;

                if (rangoTramo > 0)
                {
                    decimal montoTramo = rangoTramo * valorUIT;
                    decimal tasa = tramo.TasaPorcentaje / 100m;
                    impuestoAnual += montoTramo * tasa;
                    acumuladoUIT += rangoTramo;
                }

                if (baseImponibleUIT <= limiteSuperior)
                    break;
            }

            impuestoRentaMensual = Math.Round(impuestoAnual / 12, 2);
        }


        public void CalcularTotales()
        {
            totalIngresos = remuneracionBruta + otrosIngresos;
            totalDescuentos = aporteONP + descuentoAFP + impuestoRentaMensual +
                              tardanza.TardanzaValorDescuento + Falta.FaltaValorDescuento + descuentoAdelantos;
            netoPagar = totalIngresos - totalDescuentos;
            
            
            Trabajador trabajadorDeTardanza = tardanza.Trabajador;
            Trabajador trabajadorDeFalta = falta.Trabajador;    

            Console.WriteLine($"El descuento por tardanza pertenece al trabajador: {trabajadorDeTardanza.TrabajadorId}");
            Console.WriteLine($"El descuento por falta pertenece al trabajador: {trabajadorDeFalta.TrabajadorId}");
        }
    }
}

    
