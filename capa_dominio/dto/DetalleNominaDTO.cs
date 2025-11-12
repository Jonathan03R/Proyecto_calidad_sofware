using System;

namespace capa_dominio.dto
{
    public class DetalleNominaDTO
    {
        public int NominaId { get; set; }
        public int TrabajadorId { get; set; }
        public decimal RemuneracionBruta { get; set; }
        public decimal SueldoBasico { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal BonosRegulares { get; set; }
        public decimal OtrosIngresos { get; set; }
        public string SistemaPensionAplicado { get; set; }
        public decimal AporteEssalud { get; set; }
        public decimal AporteOnp { get; set; }
        public decimal DescuentoAfp { get; set; }
        public decimal RemuneracionAcumuladaAnual { get; set; }
        public decimal BaseImponibleAnual { get; set; }
        public decimal ImpuestoRentaAnual { get; set; }
        public decimal ImpuestoRentaMensual { get; set; }
        public decimal UitValor { get; set; }
        public decimal Deduccion7Uit { get; set; }
        public decimal DescuentoTardanzas { get; set; }
        public decimal DescuentoFaltas { get; set; }
        public decimal DescuentoAdelantos { get; set; }
        public decimal OtrosDescuentos { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal NetoPagar { get; set; }
        public bool TieneErrores { get; set; }
        public string MensajeError { get; set; }

        // 🔒 Constructor requerido (obliga a pasar todos los datos)
        public DetalleNominaDTO(
            int nominaId,
            int trabajadorId,
            decimal remuneracionBruta,
            decimal sueldoBasico,
            decimal asignacionFamiliar,
            decimal horasExtras,
            decimal bonosRegulares,
            decimal otrosIngresos,
            string sistemaPensionAplicado,
            decimal aporteEssalud,
            decimal aporteOnp,
            decimal descuentoAfp,
            decimal remuneracionAcumuladaAnual,
            decimal baseImponibleAnual,
            decimal impuestoRentaAnual,
            decimal impuestoRentaMensual,
            decimal uitValor,
            decimal deduccion7Uit,
            decimal descuentoTardanzas,
            decimal descuentoFaltas,
            decimal descuentoAdelantos,
            decimal otrosDescuentos,
            decimal totalIngresos,
            decimal totalDescuentos,
            decimal netoPagar,
            bool tieneErrores,
            string mensajeError)
        {
            NominaId = nominaId;
            TrabajadorId = trabajadorId;
            RemuneracionBruta = remuneracionBruta;
            SueldoBasico = sueldoBasico;
            AsignacionFamiliar = asignacionFamiliar;
            HorasExtras = horasExtras;
            BonosRegulares = bonosRegulares;
            OtrosIngresos = otrosIngresos;
            SistemaPensionAplicado = sistemaPensionAplicado ?? throw new ArgumentNullException(nameof(sistemaPensionAplicado));
            AporteEssalud = aporteEssalud;
            AporteOnp = aporteOnp;
            DescuentoAfp = descuentoAfp;
            RemuneracionAcumuladaAnual = remuneracionAcumuladaAnual;
            BaseImponibleAnual = baseImponibleAnual;
            ImpuestoRentaAnual = impuestoRentaAnual;
            ImpuestoRentaMensual = impuestoRentaMensual;
            UitValor = uitValor;
            Deduccion7Uit = deduccion7Uit;
            DescuentoTardanzas = descuentoTardanzas;
            DescuentoFaltas = descuentoFaltas;
            DescuentoAdelantos = descuentoAdelantos;
            OtrosDescuentos = otrosDescuentos;
            TotalIngresos = totalIngresos;
            TotalDescuentos = totalDescuentos;
            NetoPagar = netoPagar;
            TieneErrores = tieneErrores;
            MensajeError = mensajeError ?? string.Empty;
        }
    }
}
