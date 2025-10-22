using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{
    public class DetalleNominaDTO
    {
        private int nominaId;
        private int trabajadorId;
        private decimal remuneracionBruta;
        private decimal sueldoBasico;
        private decimal asignacionFamiliar;
        private decimal horasExtras;
        private decimal bonosRegulares;
        private decimal otrosIngresos;
        private string sistemaPensionAplicado;
        private decimal aporteEssalud;
        private decimal aporteOnp;
        private decimal descuentoAfp;
        private decimal remuneracionAcumuladaAnual;
        private decimal baseImponibleAnual;
        private decimal impuestoRentaAnual;
        private decimal impuestoRentaMensual;
        private decimal uitValor;
        private decimal deduccion7Uit;
        private decimal descuentoTardanzas;
        private decimal descuentoFaltas;
        private decimal descuentoAdelantos;
        private decimal otrosDescuentos;
        private decimal totalIngresos;
        private decimal totalDescuentos;
        private decimal netoPagar;
        private bool tieneErrores;
        private string mensajeError;

        public int NominaId { get => nominaId; set => nominaId = value; }
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public decimal RemuneracionBruta { get => remuneracionBruta; set => remuneracionBruta = value; }
        public decimal SueldoBasico { get => sueldoBasico; set => sueldoBasico = value; }
        public decimal AsignacionFamiliar { get => asignacionFamiliar; set => asignacionFamiliar = value; }
        public decimal HorasExtras { get => horasExtras; set => horasExtras = value; }
        public decimal BonosRegulares { get => bonosRegulares; set => bonosRegulares = value; }
        public decimal OtrosIngresos { get => otrosIngresos; set => otrosIngresos = value; }
        public string SistemaPensionAplicado { get => sistemaPensionAplicado; set => sistemaPensionAplicado = value; }
        public decimal AporteEssalud { get => aporteEssalud; set => aporteEssalud = value; }
        public decimal AporteOnp { get => aporteOnp; set => aporteOnp = value; }
        public decimal DescuentoAfp { get => descuentoAfp; set => descuentoAfp = value; }
        public decimal RemuneracionAcumuladaAnual { get => remuneracionAcumuladaAnual; set => remuneracionAcumuladaAnual = value; }
        public decimal BaseImponibleAnual { get => baseImponibleAnual; set => baseImponibleAnual = value; }
        public decimal ImpuestoRentaAnual { get => impuestoRentaAnual; set => impuestoRentaAnual = value; }
        public decimal ImpuestoRentaMensual { get => impuestoRentaMensual; set => impuestoRentaMensual = value; }
        public decimal UitValor { get => uitValor; set => uitValor = value; }
        public decimal Deduccion7Uit { get => deduccion7Uit; set => deduccion7Uit = value; }
        public decimal DescuentoTardanzas { get => descuentoTardanzas; set => descuentoTardanzas = value; }
        public decimal DescuentoFaltas { get => descuentoFaltas; set => descuentoFaltas = value; }
        public decimal DescuentoAdelantos { get => descuentoAdelantos; set => descuentoAdelantos = value; }
        public decimal OtrosDescuentos { get => otrosDescuentos; set => otrosDescuentos = value; }
        public decimal TotalIngresos { get => totalIngresos; set => totalIngresos = value; }
        public decimal TotalDescuentos { get => totalDescuentos; set => totalDescuentos = value; }
        public decimal NetoPagar { get => netoPagar; set => netoPagar = value; }
        public bool TieneErrores { get => tieneErrores; set => tieneErrores = value; }
        public string MensajeError { get => mensajeError; set => mensajeError = value; }
    }
}