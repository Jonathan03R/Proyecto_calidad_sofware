using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;

namespace capa_pruebas
{
    [TestClass]
    public class HoraTrabajadaTests
    {
        // ===============================================================
        // 🔥 PRUEBA 1: CÁLCULO COMPLETO DEL TRABAJADOR MENSUAL
        // ===============================================================
        [TestMethod]
        public void CalcularInformacionCompletaContratoMensual()
        {
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,       // salario mensual
                ContratoHorasSemanales = 48,   // jornada semanal
                TipoSalario = new TipoSalario { TipoSalarioNombre = "Mensual" }
            };

            contrato.CalcularValorPorHora();   // tarifa = salario / 240

            // Cálculos adicionales
            decimal sueldoDiario = Math.Round(contrato.ContratoSalario / 30m, 2);
            decimal jornadaDiaria = Math.Round(contrato.ContratoHorasSemanales.Value / 6m, 2);

            Console.WriteLine("==================================================");
            Console.WriteLine("          INFORMACIÓN COMPLETA DEL CONTRATO       ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Salario mensual     : {contrato.ContratoSalario} soles");
            Console.WriteLine($"Salario diario       : {sueldoDiario} soles (Salario / 30 días)");
            Console.WriteLine($"Jornada diaria       : {jornadaDiaria} horas (Horas semanales / 6 días)");
            Console.WriteLine($"Tarifa por hora      : {contrato.ValorHoraCalculado} soles (Salario / 240)");
            Console.WriteLine("==================================================");
        }


        // ===============================================================
        // 🔥 PRUEBA 2: Tardanza de 1 hora → descuento = tarifaHora
        // ===============================================================
        [TestMethod]
        public void CalcularDescuentoTardanza_ConTardanzaDeUnaHora()
        {
            var contrato = new Contrato
            {
                ContratoSalario = 1200m,
                ContratoHorasSemanales = 48
            };

            contrato.CalcularValorPorHora(); // tarifa = 1200/240 = 5

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7 // trabajó 7/8 → tardanza 1 hora
            };

            decimal result = horaTrabajada.CalcularDescuentoTardanza();

            Console.WriteLine("==================================================");
            Console.WriteLine("          DESCUENTO POR TARDANZA (1 hora)         ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Salario mensual     : {contrato.ContratoSalario}");
            Console.WriteLine($"Tarifa por hora      : {contrato.ValorHoraCalculado}");
            Console.WriteLine($"Horas trabajadas     : 7/8 (tardanza de 1 hora)");
            Console.WriteLine($"Descuento aplicado   : {result} soles");
            Console.WriteLine("==================================================");

            Assert.AreEqual(5m, result);
        }


        // ===============================================================
        // 🔥 PRUEBA 3: Tardanza de 2 horas
        // ===============================================================
        [TestMethod]
        public void CalcularDescuentoTardanza_DosHoras()
        {
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,   // tarifa = 12.5
                ContratoHorasSemanales = 48
            };

            contrato.CalcularValorPorHora();

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 6 // faltan 2 horas
            };

            decimal result = horaTrabajada.CalcularDescuentoTardanza();

            Console.WriteLine("==================================================");
            Console.WriteLine("          DESCUENTO POR TARDANZA (2 horas)        ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Tarifa por hora      : {contrato.ValorHoraCalculado}");
            Console.WriteLine($"Horas tardanza       : 2");
            Console.WriteLine($"Descuento aplicado   : {result} soles");
            Console.WriteLine("==================================================");

            Assert.AreEqual(25m, result);
        }


        // ===============================================================
        // 🔥 PRUEBA 4: 12 minutos tarde → 0.2 horas
        // ===============================================================
        [TestMethod]
        public void CalcularDescuentoTardanza_12MinutosTarde()
        {
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
                ContratoHorasSemanales = 48
            };

            contrato.CalcularValorPorHora(); // tarifa = 12.5

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7.8m // tardó 12 min (0.2h)
            };

            var descuento = horaTrabajada.CalcularDescuentoTardanza();

            Console.WriteLine("==================================================");
            Console.WriteLine("      DESCUENTO POR TARDANZA (12 minutos tarde)   ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Horas trabajadas     : 7.8/8");
            Console.WriteLine($"Horas tardanza       : 0.2");
            Console.WriteLine($"Tarifa por hora      : {contrato.ValorHoraCalculado}");
            Console.WriteLine($"Descuento aplicado   : {descuento}");
            Console.WriteLine("==================================================");

            Assert.AreEqual(2.5m, descuento);
        }
    }
}
