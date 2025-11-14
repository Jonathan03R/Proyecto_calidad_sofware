using System;
using System.Collections.Generic;
using System.Linq;
using capa_dominio;
using capa_dominio.dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace capa_dominio.Tests
{
    [TestClass()]
    public class DetalleNominaTests
    {
        [TestMethod()]
        public void CalcularDescuentoFaltasTest()
        { 
            // Contrato base
            var contrato = new Contrato
            {
                ContratoSalario = 3000m,
            };

            // Periodo de nómina (1 al 31 de octubre)
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 10, 2),
                PeriodoFechaFin = new DateTime(2025, 10, 7)
            };

            // Nomina asociada al periodo
            var nomina = new Nomina
            {
                Periodo = periodo
            };

            // Detalle de nómina con contrato y nómina
            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                Nomina = nomina
            };

            // Días trabajados (faltó solo 1 día hábil)
            var horasTrabajadas = new List<HoraTrabajada>
        {
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 2), HorasNormales = 8 },//jueves
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 3), HorasNormales = 8 },/// viernes
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 4), HorasNormales = 8 },// sabado
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 5), HorasNormales = 0 },// no se cuenta
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 6), HorasNormales = 0 },//lunes
            new HoraTrabajada { Fecha = new DateTime(2025, 10, 7), HorasNormales = 0 },//martes
           
        };

            detalle.HorasTrabajadas = horasTrabajadas;

            // Act
            detalle.CalcularDescuentoFaltas();

            // sueldo diario = 3000 / 30 = 100
            // si faltó un día hábil, el descuento debe ser 100
            Assert.AreEqual(200m, detalle.DescuentoFaltas);
        }
        [TestMethod]
        public void CalcularDescuentoFaltas_HorasTrabajadasNull_DescuentoCero()
        {
            var contrato = new Contrato { ContratoSalario = 1500m };
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 5)
            };
            var nomina = new Nomina { Periodo = periodo };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                Nomina = nomina,
                HorasTrabajadas = null
            };

            detalle.CalcularDescuentoFaltas();

            Assert.AreEqual(0m, detalle.DescuentoFaltas);
        }

        [TestMethod]
        public void CalcularPagoTotalHorasExtrasTest()
        {
            // contrato con tarifa por hora
            var contrato = new Contrato
            {
                ContratoTarifaHora = 10m
            };

            // tabla de multiplicadores
            var tipos = new List<TipoHoraExtra>
            {
                new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "ADICIONALES", TiposHorasExtrasMultiplicador = 0.35m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "SABADO", TiposHorasExtrasMultiplicador = 0.50m, TiposHorasExtrasEstado = 'A' },
                new TipoHoraExtra { TiposHorasExtrasCodigo = "DOMINGO", TiposHorasExtrasMultiplicador = 1.00m, TiposHorasExtrasEstado = 'A' }
            };

            // escenario de varias fechas
            var horas = new List<HoraTrabajada>
            {
                // lunes -> 3 horas extras (2 primeras + 1 adicional)
                new HoraTrabajada
                {
                    Fecha = new DateTime(2025,11,3),
                    HorasExtras = 3,

                },

                // sábado -> 1 hora extra con recargo sábado
                new HoraTrabajada
                {
                    Fecha = new DateTime(2025,11,8),
                    HorasExtras = 1
                },

                // domingo -> 2 horas extras con 100% adicional
                new HoraTrabajada
                {
                    Fecha = new DateTime(2025,11,9),
                    HorasExtras = 2
                },

                // día normal sin extras
                new HoraTrabajada
                {
                    Fecha = new DateTime(2025,11,10),
                    HorasExtras = 0
                }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = horas,
                TiposHorasExtras = tipos
            };

            // act
            detalle.CalcularPagoTotalHorasExtras();

            /*
                CALCULOS ESPERADOS:

                LUNES (3 extras):
                    primeras 2: 2 * 10 * 1.25 = 25
                    adicionales: 1 * 10 * 1.35 = 13.5
                    total lunes = 38.5

                SÁBADO (1 extra):
                    1 * 10 * 1.50 = 15

                DOMINGO (2 extras):
                    2 * 10 * 2.00 = 40

                TOTAL ESPERADO = 38.5 + 15 + 40 = 93.5
            */

            Assert.AreEqual(93.5m, detalle.HorasExtras);
        }
        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_HorasTrabajadasNulasODesocupadas_DebeDevolverCero()
        {
            // Arrange
            var contrato = new Contrato { ContratoTarifaHora = 10m };
            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = null,
                TiposHorasExtras = new List<TipoHoraExtra>
        {
            new TipoHoraExtra { TiposHorasExtrasCodigo = "PRIMERAS2", TiposHorasExtrasMultiplicador = 0.25m }
        }
            };

            // Act
            detalle.CalcularPagoTotalHorasExtras();

            // Assert
            Assert.AreEqual(0m, detalle.HorasExtras);

            // También probar con lista vacía
            detalle.HorasTrabajadas = new List<HoraTrabajada>();
            detalle.CalcularPagoTotalHorasExtras();
            Assert.AreEqual(0m, detalle.HorasExtras);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularPagoTotalHorasExtras_TiposHorasExtrasNulosODesocupados_DebeLanzarExcepcion()
        {
            // Arrange
            var contrato = new Contrato { ContratoTarifaHora = 10m };
            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = new List<HoraTrabajada> { new HoraTrabajada { HorasExtras = 2 } },
                TiposHorasExtras = null
            };

            // Act
            detalle.CalcularPagoTotalHorasExtras();

            // Assert se maneja por ExpectedException
        }
        [TestMethod]
        public void CalcularDescuentoTardanzas_VariosDias_DebeSumarCorrectamente()
        {
            var contrato = new Contrato
            {
                ContratoSalario = 1500m,        // sueldo mensual
                ContratoHorasSemanales = 48     // 8h por día
            };

            // jornada diaria = 8h
            // sueldo por día = 1500 / 30 = 50
            // descuento por hora = 50 / 8 = 6.25

            var horas = new List<HoraTrabajada>
            {
                new HoraTrabajada { Contrato = contrato, HorasNormales = 8m },    // 0h tardanza -> 0
                new HoraTrabajada { Contrato = contrato, HorasNormales = 7.5m },  // 0.5h -> 0.5 * 6.25 = 3.125 -> 3.13
                new HoraTrabajada { Contrato = contrato, HorasNormales = 6m }     // 2h -> 2 * 6.25 = 12.5
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = horas
            };

            // Act
            detalle.CalcularDescuentoTardanzas();

            // Assert
            // Total = 0 + 3.13 + 12.5 = 15.63
            Assert.AreEqual(15.63m, detalle.DescuentoTardanzas);
        }

        [TestMethod]
        public void CalcularDescuentoTardanzas_HorasNull_DebeSerCero()
        {
            var detalle = new DetalleNomina
            {
                Contrato = new Contrato { ContratoSalario = 1500m, ContratoHorasSemanales = 48 },
                HorasTrabajadas = null
            };

            detalle.CalcularDescuentoTardanzas();

            Assert.AreEqual(0m, detalle.DescuentoTardanzas);
        }

        [TestMethod]
        public void CalcularDescuentoTardanzas_HorasVacia_DebeSerCero()
        {
            var detalle = new DetalleNomina
            {
                Contrato = new Contrato { ContratoSalario = 1500m, ContratoHorasSemanales = 48 },
                HorasTrabajadas = new List<HoraTrabajada>()
            };

            detalle.CalcularDescuentoTardanzas();

            Assert.AreEqual(0m, detalle.DescuentoTardanzas);
        }


        [TestMethod()]
        public void calcularRemuneracionBrutaTest()
        {
            var detalle2 = new DetalleNomina();
            detalle2.Contrato = new Contrato { ContratoSalario = 1500 };
            detalle2.HorasExtras = 0;
            detalle2.AsignacionFamiliar = 102.5m;
            detalle2.BonosRegulares = 0;

            detalle2.calcularRemuneracionBruta();
            Assert.AreEqual(1602.5m, detalle2.RemuneracionBruta);
        }

        [TestMethod]
        public void CalcularRemuneracionBruta_TodoIncluido()
        {
            var detalle1 = new DetalleNomina();
            detalle1.Contrato = new Contrato { ContratoSalario = 1500 };
            detalle1.HorasExtras = 200;
            detalle1.AsignacionFamiliar = 102.5m;
            detalle1.BonosRegulares = 150;

            detalle1.calcularRemuneracionBruta();
            Assert.AreEqual(1952.5m, detalle1.RemuneracionBruta);
        }

        [TestMethod]
        public void CalcularRemuneracionBruta_TodoCero()
        {
            var detalle3 = new DetalleNomina();
            detalle3.Contrato = new Contrato { ContratoSalario = 1500 };
            detalle3.HorasExtras = 0;
            detalle3.AsignacionFamiliar = 0;
            detalle3.BonosRegulares = 0;

            detalle3.calcularRemuneracionBruta();
            Assert.AreEqual(1500m, detalle3.RemuneracionBruta);
        }


        [TestMethod]
        public void CalculoAsignacionFamiliar_ConAsignacion_Test()
        {
            // Arrange
            var contrato = new Contrato
            {
                ContratoSalario = 1500m
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato
            };

            // Act
            var resultado = detalle.CalculoAsignacionFamiliar(true);

            // Assert
            Assert.AreEqual(150.00m, resultado);
        }
        [TestMethod]
        public void CalculoAsignacionFamiliar_SinAsignacion_Test()
        {
            // Arrange
            var contrato = new Contrato
            {
                ContratoSalario = 1500m
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato
            };

            // Act
            var resultado = detalle.CalculoAsignacionFamiliar(false);

            // Assert
            Assert.AreEqual(0m, resultado);
        }
        [TestMethod]
        public void CalculoAsignacionFamiliar_ReemplazaValorPrevio_Test()
        {
            // Arrange
            var contrato = new Contrato
            {
                ContratoSalario = 1500m
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato
            };

            // simula un valor viejo que NO debe mantenerse
            detalle.AsignacionFamiliar = 800m;

            // Act
            var resultado = detalle.CalculoAsignacionFamiliar(true);

            // Assert
            Assert.AreEqual(150.00m, resultado);
        }

        [TestMethod]
        public void CalcularSistemaPensiones_ONP_Test()
        {
            // Arrange
            var contrato = new Contrato
            {
                TipoPension = new TipoPension { TipoPensionId = 1, Nombre = "ONP" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                RemuneracionBruta = 2000m
            };

            // Act
            detalle.CalcularSistemaPensiones();

            // Assert
            Assert.AreEqual(260.00m, detalle.AporteONP); // 13% de 2000
            Assert.AreEqual(0m, detalle.DescuentoAFP);
            Assert.AreEqual("ONP", detalle.SistemasPensionAplicado);
        }
        [TestMethod]
        public void CalcularSistemaPensiones_AFP_Test()
        {
            // Arrange
            var contrato = new Contrato
            {
                TipoPension = new TipoPension { TipoPensionId = 3, Nombre = "AFP Prima" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                RemuneracionBruta = 1500m
            };

            // Act
            detalle.CalcularSistemaPensiones();

            // Assert
            Assert.AreEqual(150.00m, detalle.DescuentoAFP); // 10% de 1500
            Assert.AreEqual(0m, detalle.AporteONP);
            Assert.AreEqual("AFP Prima", detalle.SistemasPensionAplicado);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularSistemaPensiones_TipoNoDefinido_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                Contrato = new Contrato(), // TipoPension = null → ERROR
                RemuneracionBruta = 1000m
            };

            // Act
            detalle.CalcularSistemaPensiones();
        }

        [TestMethod]
        public void CalcularAporteEssalud_CalculoCorrecto_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 1500m
            };

            var parametroEssalud = new Parametro
            {
                ParametroValor = 0.09m // 9%
            };

            // Act
            detalle.CalcularAporteEssalud(parametroEssalud);

            // Assert
            Assert.AreEqual(135.00m, detalle.AporteEssalud); // 1500 * 0.09
        }
        [TestMethod]
        public void CalcularAporteEssalud_RemuneracionCero_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 0m
            };

            var parametroEssalud = new Parametro
            {
                ParametroValor = 0.09m
            };

            // Act
            detalle.CalcularAporteEssalud(parametroEssalud);

            // Assert
            Assert.AreEqual(0m, detalle.AporteEssalud);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalcularAporteEssalud_ParametroNull_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 1500m
            };

            // Act
            detalle.CalcularAporteEssalud(null);

            // Assert → manejado por ExpectedException
        }

        [TestMethod]
        public void CalcularImpuestoRentaQuinta_NoPagaImpuesto_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 1500m
            };

            decimal valorUIT = 5150m;
            var tramos = new List<ImpuestoRentaTramo>
    {
        new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 }
    };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.AreEqual(0m, detalle.ImpuestoRentaMensual);
        }
        [TestMethod]
        public void CalcularImpuestoRentaQuinta_UnSoloTramo_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 6000m
            };

            decimal valorUIT = 5150m;

            var tramos = new List<ImpuestoRentaTramo>
    {
        new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
        new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 }
    };

            // ACT
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // CALCULO MANUAL:
            // Remuneración anual = 6000*12 = 72000
            // Deducción = 7 UIT = 36 050
            // Base imponible anual = 72000 - 36050 = 35950
            // Base imponible en UIT = 35950 / 5150 = 6.98 UIT
            // Solo tramo 1: 5 UIT * 5150 = 25 750 * 8% = 2060
            // EXCEDENTE tramo 2: (6.98 - 5) = 1.98 UIT ≈ 10197 * 14% = 1428
            // Impuesto anual ≈ 3488
            // Mensual = 3488/12 = 290.67

            // Assert
            Assert.AreEqual(290.67m, detalle.ImpuestoRentaMensual);
        }

        [TestMethod()]
        public void CalcularImpuestoRentaQuinta_MultiplesTramos_Test()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 10000m
            };

            decimal valorUIT = 5150m;

            var tramos = new List<ImpuestoRentaTramo>
    {
        new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
        new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 },
        new ImpuestoRentaTramo { NumeroTramo = 3, LimiteInferiorUIT = 20, LimiteSuperiorUIT = 35, TasaPorcentaje = 17 }
    };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert (valor correcto generado por la fórmula real)
            Assert.AreEqual(850.67m, detalle.ImpuestoRentaMensual);
        }

        [TestMethod()]
        public void CalcularTotalesTest()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 2500m,

                AporteONP = 200m,
                DescuentoAFP = 0m,
                ImpuestoRentaMensual = 150m,
                DescuentoFaltas = 50m,
                DescuentoAdelantos = 100m
            };

            // Cálculo esperado:
            // totalIngresos = 2500
            // totalDescuentos = 200 + 0 + 150 + 50 + 100 = 500
            // netoPagar = 2500 - 500 = 2000

            decimal ingresosEsperados = 2500m;
            decimal descuentosEsperados = 500m;
            decimal netoEsperado = 2000m;

            // Act
            detalle.CalcularTotales();

            // Assert
            Assert.AreEqual(ingresosEsperados, detalle.TotalIngresos);
            Assert.AreEqual(descuentosEsperados, detalle.TotalDescuentos);
            Assert.AreEqual(netoEsperado, detalle.NetoPagar);
        }


    }

}