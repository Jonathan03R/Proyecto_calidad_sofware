using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.Tests
{
    [TestClass()]
    public class HoraTrabajadaTests
    {

        [TestMethod()]
        public void CalcularDescuentoTardanza_ConTardanzaDeUnaHora_ReturnsCinco()
        {
            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 1200m
                //dia 40
                //tarifa por hora 5
            };
            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7
            };
            decimal result = horaTrabajada.CalcularDescuentoTardanza();
            decimal expected = 5;
            Assert.AreEqual(expected, result);
        }



        /// <summary>
        /// definir más pruebas para el método CalcularDescuentoTardanza
        /// </summary>

        [TestMethod]
        public void CalcularDescuentoTardanzaTest()
        {
            // Contrato con salario mensual de 3000 y 48 horas semanales
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
                ContratoHorasSemanales = 48
            };

            // Jornada diaria = 48 / 6 = 8 horas
            // Sueldo diario = 3000 / 30 = 100 soles
            // Descuento por hora = 100 / 8 = 12.5 soles

            // Caso: trabajó solo 6 horas (faltaron 2 horas)
            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 6
            };

            // Act
            var descuento = horaTrabajada.CalcularDescuentoTardanza();

            // Assert
            // 2 horas * 12.5 = 25 soles de descuento
            Assert.AreEqual(25m, descuento);
        }

        [TestMethod]
        public void CalcularDescuentoTardanza_12MinutosTarde_DebeCalcularCorrectamente()
        {
            // Arrange
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
                ContratoHorasSemanales = 48
            };

            // Jornada diaria = 8 horas
            // Sueldo diario = 100 soles
            // Descuento por hora = 12.5 soles
            // Tardanza: 12 minutos (0.2 horas)

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7.8m // llegó 12 minutos tarde
            };

            // Act
            var descuento = horaTrabajada.CalcularDescuentoTardanza();

            // Assert
            // 0.2 horas * 12.5 soles = 2.5 soles de descuento
            Assert.AreEqual(2.5m, descuento);
        }


        /// <summary>
        /// Test para el método CalcularPagoHorasExtras
        /// </summary>

        [TestMethod()]
        public void CalcularPagoHorasExtras_SabadoTest()
        {
            var contrato = new Contrato { ContratoTarifaHora = 10m };

            var tiposHorasExtras = new List<TipoHoraExtra>
    {
        new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
    };

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 4), // sábado
                HorasExtras = 1,
                TiposHorasExtras = tiposHorasExtras
            };

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            // 1 hora * 10 * (1 + 0.5) = 15
            Assert.AreEqual(15m, pagoExtras);
        }


        [TestMethod()]
        public void CalcularPagoHorasExtras_DomingoTest()
        {
            var contrato = new Contrato { ContratoTarifaHora = 10m };

            var tiposHorasExtras = new List<TipoHoraExtra>
    {
        new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
    };

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 5), // domingo
                HorasExtras = 2,
                TiposHorasExtras = tiposHorasExtras
            };

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            // 2 horas * 10 * (1 + 1) = 40
            Assert.AreEqual(40m, pagoExtras);
        }


    }
}