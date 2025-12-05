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
            Console.WriteLine("=== PRUEBA: CalcularDescuentoTardanza_ConTardanzaDeUnaHora_ReturnsCinco ===");

            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 1200m
                //dia 40
                //tarifa por hora 5
            };
            Console.WriteLine($"Contrato - Horas semanales: {contrato.ContratoHorasSemanales}, Salario: {contrato.ContratoSalario}");

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7
            };
            Console.WriteLine($"Hora trabajada - Horas normales: {horaTrabajada.HorasNormales}");

            decimal result = horaTrabajada.CalcularDescuentoTardanza();
            decimal expected = 5;

            Console.WriteLine($"Descuento por tardanza calculado: {result}");
            Console.WriteLine($"Descuento esperado: {expected}");

            Assert.AreEqual(expected, result);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }



        /// <summary>
        /// definir más pruebas para el método CalcularDescuentoTardanza
        /// </summary>

        [TestMethod]
        public void CalcularDescuentoTardanzaTest()
        {
            Console.WriteLine("=== PRUEBA: CalcularDescuentoTardanzaTest ===");

            // Contrato con salario mensual de 3000 y 48 horas semanales
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
                ContratoHorasSemanales = 48
            };
            Console.WriteLine($"Contrato - Salario: {contrato.ContratoSalario}, Horas semanales: {contrato.ContratoHorasSemanales}");

            // Jornada diaria = 48 / 6 = 8 horas
            // Sueldo diario = 3000 / 30 = 100 soles
            // Descuento por hora = 100 / 8 = 12.5 soles

            // Caso: trabajó solo 6 horas (faltaron 2 horas)
            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 6
            };
            Console.WriteLine($"Hora trabajada - Horas normales: {horaTrabajada.HorasNormales}");
            Console.WriteLine($"Horas esperadas: 8, Horas faltantes: 2");

            // Act
            var descuento = horaTrabajada.CalcularDescuentoTardanza();

            Console.WriteLine($"Descuento por tardanza calculado: {descuento}");
            Console.WriteLine($"Descuento esperado: 25");

            // Assert
            // 2 horas * 12.5 = 25 soles de descuento
            Assert.AreEqual(25m, descuento);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }

        [TestMethod]
        public void CalcularDescuentoTardanza_12MinutosTarde_DebeCalcularCorrectamente()
        {
            Console.WriteLine("=== PRUEBA: CalcularDescuentoTardanza_12MinutosTarde_DebeCalcularCorrectamente ===");

            // Arrange
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
                ContratoHorasSemanales = 48
            };
            Console.WriteLine($"Contrato - Salario: {contrato.ContratoSalario}, Horas semanales: {contrato.ContratoHorasSemanales}");

            // Jornada diaria = 8 horas
            // Sueldo diario = 100 soles
            // Descuento por hora = 12.5 soles
            // Tardanza: 12 minutos (0.2 horas)

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                HorasNormales = 7.8m // llegó 12 minutos tarde
            };
            Console.WriteLine($"Hora trabajada - Horas normales: {horaTrabajada.HorasNormales}");
            Console.WriteLine($"Tardanza: 12 minutos (0.2 horas)");

            // Act
            var descuento = horaTrabajada.CalcularDescuentoTardanza();

            Console.WriteLine($"Descuento por tardanza calculado: {descuento}");
            Console.WriteLine($"Descuento esperado: 2.5");

            // Assert
            // 0.2 horas * 12.5 soles = 2.5 soles de descuento
            Assert.AreEqual(2.5m, descuento);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }


        /// <summary>
        /// Test para el método CalcularPagoHorasExtras
        /// </summary>

        [TestMethod()]
        public void CalcularPagoHorasExtras_SabadoTest()
        {
            Console.WriteLine("=== PRUEBA: CalcularPagoHorasExtras_SabadoTest ===");

            var contrato = new Contrato { ContratoTarifaHora = 10m };
            Console.WriteLine($"Contrato - Tarifa por hora: {contrato.ContratoTarifaHora}");

            var tiposHorasExtras = new List<TipoHoraExtra>
            {
                new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
            };

            Console.WriteLine("Tipos de horas extras configurados:");
            foreach (var tipo in tiposHorasExtras)
            {
                Console.WriteLine($"  - {tipo.TiposHorasExtrasCodigo}: Multiplicador {tipo.TiposHorasExtrasMultiplicador}");
            }

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 4), // sábado
                HorasExtras = 1,
                TiposHorasExtras = tiposHorasExtras
            };
            Console.WriteLine($"Hora trabajada - Fecha: {horaTrabajada.Fecha.ToShortDateString()} (Sábado), Horas extras: {horaTrabajada.HorasExtras}");

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            Console.WriteLine($"Pago horas extras calculado: {pagoExtras}");
            Console.WriteLine($"Pago esperado: 15 (1 hora * 10 * (1 + 0.5))");

            // 1 hora * 10 * (1 + 0.5) = 15
            Assert.AreEqual(15m, pagoExtras);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }


        [TestMethod()]
        public void CalcularPagoHorasExtras_DomingoTest()
        {
            Console.WriteLine("=== PRUEBA: CalcularPagoHorasExtras_DomingoTest ===");

            var contrato = new Contrato { ContratoTarifaHora = 10m };
            Console.WriteLine($"Contrato - Tarifa por hora: {contrato.ContratoTarifaHora}");

            var tiposHorasExtras = new List<TipoHoraExtra>
            {
                new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
            };

            Console.WriteLine("Tipos de horas extras configurados:");
            foreach (var tipo in tiposHorasExtras)
            {
                Console.WriteLine($"  - {tipo.TiposHorasExtrasCodigo}: Multiplicador {tipo.TiposHorasExtrasMultiplicador}");
            }

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 5), // domingo
                HorasExtras = 2,
                TiposHorasExtras = tiposHorasExtras
            };
            Console.WriteLine($"Hora trabajada - Fecha: {horaTrabajada.Fecha.ToShortDateString()} (Domingo), Horas extras: {horaTrabajada.HorasExtras}");

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            Console.WriteLine($"Pago horas extras calculado: {pagoExtras}");
            Console.WriteLine($"Pago esperado: 40 (2 horas * 10 * (1 + 1))");

            // 2 horas * 10 * (1 + 1) = 40
            Assert.AreEqual(40m, pagoExtras);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }

        [TestMethod()]
        public void CalcularPagoHorasExtras_DiaSemanaTest()
        {
            Console.WriteLine("=== PRUEBA: CalcularPagoHorasExtras_DiaSemanaTest ===");

            var contrato = new Contrato { ContratoTarifaHora = 10m };
            Console.WriteLine($"Contrato - Tarifa por hora: {contrato.ContratoTarifaHora}");

            var tiposHorasExtras = new List<TipoHoraExtra>
            {
                new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
            };

            Console.WriteLine("Tipos de horas extras configurados:");
            foreach (var tipo in tiposHorasExtras)
            {
                Console.WriteLine($"  - {tipo.TiposHorasExtrasCodigo}: Multiplicador {tipo.TiposHorasExtrasMultiplicador}");
            }

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 1), // miércoles
                HorasExtras = 3,
                TiposHorasExtras = tiposHorasExtras
            };
            Console.WriteLine($"Hora trabajada - Fecha: {horaTrabajada.Fecha.ToShortDateString()} (Miércoles), Horas extras: {horaTrabajada.HorasExtras}");

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            Console.WriteLine($"Pago horas extras calculado: {pagoExtras}");
            Console.WriteLine($"Cálculo esperado: 2 primeras horas * 10 * 1.25 = 25, 1 hora adicional * 10 * 1.35 = 13.5, Total = 38.5");

            // 2 primeras horas * 10 * 1.25 = 25
            // 1 hora adicional * 10 * 1.35 = 13.5
            // Total = 38.5
            Assert.AreEqual(38.5m, pagoExtras);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }

        [TestMethod()]
        public void CalcularPagoHorasExtras_SinHorasExtras_Test()
        {
            Console.WriteLine("=== PRUEBA: CalcularPagoHorasExtras_SinHorasExtras_Test ===");

            var contrato = new Contrato { ContratoTarifaHora = 10m };
            Console.WriteLine($"Contrato - Tarifa por hora: {contrato.ContratoTarifaHora}");

            var tiposHorasExtras = new List<TipoHoraExtra>
    {
        new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
        new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.0m, TiposHorasExtrasEstado = 'A' }
    };

            Console.WriteLine("Tipos de horas extras configurados:");
            foreach (var tipo in tiposHorasExtras)
            {
                Console.WriteLine($"  - {tipo.TiposHorasExtrasCodigo}: Multiplicador {tipo.TiposHorasExtrasMultiplicador}");
            }

            var horaTrabajada = new HoraTrabajada
            {
                Contrato = contrato,
                Fecha = new DateTime(2025, 10, 1),
                HorasExtras = 0, // Sin horas extras
                TiposHorasExtras = tiposHorasExtras
            };
            Console.WriteLine($"Hora trabajada - Horas extras: {horaTrabajada.HorasExtras}");

            var pagoExtras = horaTrabajada.CalcularPagoHorasExtras();

            Console.WriteLine($"Pago horas extras calculado: {pagoExtras}");
            Console.WriteLine($"Pago esperado: 0");

            Assert.AreEqual(0m, pagoExtras);

            Console.WriteLine("=== PRUEBA EXITOSA ===\n");
        }

        [TestMethod]
        public void CalcularPagoHorasExtras_DiaNormal_3Horas()
        {
            // datos base
            var contrato = new Contrato
            {
                ContratoTarifaHora = 6.25m
            };

            var tiposHorasExtras = new List<TipoHoraExtra>
            {
                new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m,  TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m ,  TiposHorasExtrasEstado = 'A'},
                new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m,  TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.00m,  TiposHorasExtrasEstado = 'A' }
            };

            var horaTrabajada = new HoraTrabajada
            {
                Fecha = new DateTime(2025, 10, 2), // jueves
                HorasExtras = 1,
                Contrato = contrato,
                TiposHorasExtras = tiposHorasExtras
            };

            // ejecucion
            var resultado = horaTrabajada.CalcularPagoHorasExtras();

            // resultado esperado:
            // 2 horas * 10 * 1.25 = 25
            // 1 hora  * 10 * 1.35 = 13.5
            // total = 38.50
            decimal esperado = 38.50m;

            Assert.AreEqual(esperado, resultado);
        }
    }
}