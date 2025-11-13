using System;
using System.Collections.Generic;
using System.Linq;
using capa_dominio;
using capa_dominio.dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyProject.Tests
{
    [TestClass]
    public class DetalleNominaTests
    {
        // -------------------------------------------------------------
        // PRUEBA 1: Calcular Remuneración Bruta con todos los conceptos
        // -------------------------------------------------------------
        [TestMethod]
        public void CalcularRemuneracionBruta_TodosLosConceptos_DevuelveCorrecto()
        {
            // Datos de entrada
            var contrato = new Contrato { ContratoSalario = 3000m };
            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                SueldoBasico = 3000m,
                AsignacionFamiliar = 102.5m,
                HorasExtras = 150m,
                BonosRegulares = 200m,
                OtrosIngresos = 50m
            };

            // Acción
            detalle.CalcularRemuneracionBruta();

            // Resultado esperado
            decimal esperado = 3000m + 102.5m + 150m + 200m + 50m;

            // Verificación
            Assert.AreEqual(esperado, detalle.RemuneracionBruta);
        }

        // -------------------------------------------------------------
        // PRUEBA 2: Calcular Remuneración Bruta sin bonos ni extras
        // -------------------------------------------------------------
        [TestMethod]
        public void CalcularRemuneracionBruta_SinBonosNiExtras_DevuelveCorrecto()
        {
            var contrato = new Contrato { ContratoSalario = 2500m };
            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                SueldoBasico = 2500m,
                AsignacionFamiliar = 0m,
                HorasExtras = 0m,
                BonosRegulares = 0m,
                OtrosIngresos = 0m
            };

            detalle.CalcularRemuneracionBruta();

            Assert.AreEqual(2500m, detalle.RemuneracionBruta);
        }

        // -------------------------------------------------------------
        // PRUEBA 3: Calcular Sistema de Pensiones AFP (10%)
        // -------------------------------------------------------------

        [TestMethod]
        public void CalcularSistemaPensiones_ONP_Devuelve13PorCiento()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = new Contrato
                {
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = 1, // ONP
                        Nombre = "ONP"
                    }
                }
            };

            detalle.CalcularSistemaPensiones();

            decimal aporteEsperado = Math.Round(3000m * 0.13m, 2);
            Assert.AreEqual(aporteEsperado, detalle.AporteONP, "El aporte ONP no es correcto");
            Assert.AreEqual(0m, detalle.DescuentoAFP, "El descuento AFP debe ser 0 para ONP");
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void CalcularSistemaPensiones_AFP_Devuelve10PorCiento(int tipoPensionId)
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = new Contrato
                {
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = tipoPensionId,
                        Nombre = "AFP"
                    }
                }
            };

            detalle.CalcularSistemaPensiones();

            decimal descuentoEsperado = Math.Round(3000m * 0.10m, 2);
            Assert.AreEqual(descuentoEsperado, detalle.DescuentoAFP, $"El descuento AFP no es correcto para TipoPensionId={tipoPensionId}");
            Assert.AreEqual(0m, detalle.AporteONP, "El aporte ONP debe ser 0 para AFP");
        }

        [TestMethod]
        public void CalcularSistemaPensiones_Tipo6_NoHaceNada()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = new Contrato
                {
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = 6,
                        Nombre = "Otro"
                    }
                }
            };

            detalle.CalcularSistemaPensiones();

            Assert.AreEqual(0m, detalle.AporteONP, "El aporte ONP debe ser 0 para TipoPensionId=6");
            Assert.AreEqual(0m, detalle.DescuentoAFP, "El descuento AFP debe ser 0 para TipoPensionId=6");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularSistemaPensiones_TipoNoReconocido_LanzaExcepcion()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = new Contrato
                {
                    TipoPension = new TipoPension
                    {
                        TipoPensionId = 99, // inválido
                        Nombre = "Desconocido"
                    }
                }
            };

            detalle.CalcularSistemaPensiones();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularSistemaPensiones_ContratoNulo_LanzaExcepcion()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = null
            };

            detalle.CalcularSistemaPensiones();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularSistemaPensiones_TipoPensionNulo_LanzaExcepcion()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 3000m,
                Contrato = new Contrato
                {
                    TipoPension = null
                }
            };

            detalle.CalcularSistemaPensiones();
        }

        // -------------------------------------------------------------
        // PRUEBA 5: Calcular Aporte ESSALUD (9%)
        // -------------------------------------------------------------
        [TestMethod]
        public void CalcularAporteEssalud_Devuelve9PorCiento()
        {
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 4000m
            };

            var parametro = new Parametro
            {
                ParametroNombre = "ESSALUD",
                ParametroValor = 0.09m
            };

            detalle.CalcularAporteEssalud(parametro);

            decimal esperado = 360m; // 9% de 4000
            Assert.AreEqual(esperado, detalle.AporteEssalud);
        }

        // ---------------------------------------------
        // 1. Sin faltas ni tardanzas
        // ---------------------------------------------
        [TestMethod]
        public void CalcularSueldoSegunAsistencia_SinFaltasNiTardanzas()
        {
            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 3000m,
                TipoPension = new TipoPension { TipoPensionId = 2, Nombre = "AFP" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = new List<HoraTrabajada>()
            };

            DateTime inicio = new DateTime(2025, 11, 3); // lunes
            DateTime fin = new DateTime(2025, 11, 7);    // viernes

            for (int i = 0; i < 5; i++)
            {
                var dia = inicio.AddDays(i);
                detalle.HorasTrabajadas.Add(new HoraTrabajada
                {
                    Fecha = dia,
                    HorasNormales = 8m, // jornada completa
                    Contrato = contrato
                });
            }

            detalle.CalcularSueldoSegunAsistencia(inicio, fin);

            decimal sueldoPorDia = Math.Round(3000m / 30m, 2);
            Assert.AreEqual(sueldoPorDia * 5, detalle.SueldoBasico);
            Assert.AreEqual(0m, detalle.DescuentoFaltas);
            Assert.AreEqual(0m, detalle.DescuentoTardanzas);
        }

        // ---------------------------------------------
        // 2. Con faltas
        // ---------------------------------------------
        [TestMethod]
        public void CalcularSueldoSegunAsistencia_ConFaltas()
        {
            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 3000m,
                TipoPension = new TipoPension { TipoPensionId = 2, Nombre = "AFP" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = new List<HoraTrabajada>()
            };

            DateTime inicio = new DateTime(2025, 11, 3);
            DateTime fin = new DateTime(2025, 11, 7);

            // Lunes y martes trabajados completos
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio, HorasNormales = 8m, Contrato = contrato });
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(1), HorasNormales = 8m, Contrato = contrato });

            // Miércoles y jueves faltas (0 horas)
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(2), HorasNormales = 0m, Contrato = contrato });
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(3), HorasNormales = 0m, Contrato = contrato });

            // Viernes trabajado completo
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(4), HorasNormales = 8m, Contrato = contrato });

            detalle.CalcularSueldoSegunAsistencia(inicio, fin);

            decimal sueldoPorDia = Math.Round(3000m / 30m, 2);
            Assert.AreEqual(sueldoPorDia * 3, detalle.SueldoBasico); // 3 días pagados
            Assert.AreEqual(sueldoPorDia * 2, detalle.DescuentoFaltas); // 2 días faltas
            Assert.AreEqual(0m, detalle.DescuentoTardanzas);
        }

        // ---------------------------------------------
        // 3. Con tardanzas
        // ---------------------------------------------
        [TestMethod]
        public void CalcularSueldoSegunAsistencia_ConTardanzas()
        {
            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 3000m,
                TipoPension = new TipoPension { TipoPensionId = 2, Nombre = "AFP" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = new List<HoraTrabajada>()
            };

            DateTime inicio = new DateTime(2025, 11, 3);
            DateTime fin = new DateTime(2025, 11, 7);

            // Todos los días trabajados, pero martes y jueves con tardanza (4 horas)
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio, HorasNormales = 8m, Contrato = contrato });
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(1), HorasNormales = 4m, Contrato = contrato }); // tardanza
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(2), HorasNormales = 8m, Contrato = contrato });
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(3), HorasNormales = 6m, Contrato = contrato }); // tardanza
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(4), HorasNormales = 8m, Contrato = contrato });

            detalle.CalcularSueldoSegunAsistencia(inicio, fin);

            decimal sueldoPorDia = Math.Round(3000m / 30m, 2);
            decimal descuentoTardanza1 = detalle.HorasTrabajadas[1].CalcularDescuentoTardanza();
            decimal descuentoTardanza2 = detalle.HorasTrabajadas[3].CalcularDescuentoTardanza();

            Assert.AreEqual(sueldoPorDia * 5, detalle.SueldoBasico); // todos los días pagados
            Assert.AreEqual(0m, detalle.DescuentoFaltas); // no hay faltas
            Assert.AreEqual(Math.Round(descuentoTardanza1 + descuentoTardanza2, 2), detalle.DescuentoTardanzas);
        }

        // ---------------------------------------------
        // 4. Con faltas y tardanzas
        // ---------------------------------------------
        [TestMethod]
        public void CalcularSueldoSegunAsistencia_FaltasYTardanzas()
        {
            var contrato = new Contrato
            {
                ContratoHorasSemanales = 48,
                ContratoSalario = 3000m,
                TipoPension = new TipoPension { TipoPensionId = 2, Nombre = "AFP" }
            };

            var detalle = new DetalleNomina
            {
                Contrato = contrato,
                HorasTrabajadas = new List<HoraTrabajada>()
            };

            DateTime inicio = new DateTime(2025, 11, 3);
            DateTime fin = new DateTime(2025, 11, 7);

            // Lunes trabajado completo
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio, HorasNormales = 8m, Contrato = contrato });

            // Martes con tardanza
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(1), HorasNormales = 4m, Contrato = contrato });

            // Miércoles y jueves faltas
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(2), HorasNormales = 0m, Contrato = contrato });
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(3), HorasNormales = 0m, Contrato = contrato });

            // Viernes trabajado completo
            detalle.HorasTrabajadas.Add(new HoraTrabajada { Fecha = inicio.AddDays(4), HorasNormales = 8m, Contrato = contrato });

            detalle.CalcularSueldoSegunAsistencia(inicio, fin);

            decimal sueldoPorDia = Math.Round(3000m / 30m, 2);
            decimal descuentoTardanza = detalle.HorasTrabajadas[1].CalcularDescuentoTardanza();

            Assert.AreEqual(sueldoPorDia * 3, detalle.SueldoBasico); // 3 días pagados
            Assert.AreEqual(sueldoPorDia * 2, detalle.DescuentoFaltas); // 2 días faltas
            Assert.AreEqual(descuentoTardanza, detalle.DescuentoTardanzas); // 1 tardanza
        }
    }

    [TestClass]
    public class DetalleNominaImpuestoTests
    {
        private decimal valorUIT = 5000m; // Ejemplo de UIT

        [TestMethod]
        public void CalcularImpuestoRentaQuinta_BaseImponibleNegativa_DevuelveCero()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 2000m // Remuneración baja → Base imponible negativa
            };

            var tramos = new List<ImpuestoRentaTramo>
        {
            new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
            new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 },
            new ImpuestoRentaTramo { NumeroTramo = 3, LimiteInferiorUIT = 20, LimiteSuperiorUIT = null, TasaPorcentaje = 17 }
        };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.AreEqual(0m, detalle.ImpuestoRentaMensual);
        }

        [TestMethod]
        public void CalcularImpuestoRentaQuinta_UnTramo_DevuelveCorrecto()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 4000m // Base imponible dentro del primer tramo
            };

            var tramos = new List<ImpuestoRentaTramo>
        {
            new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
            new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 },
            new ImpuestoRentaTramo { NumeroTramo = 3, LimiteInferiorUIT = 20, LimiteSuperiorUIT = null, TasaPorcentaje = 17 }
        };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Cálculo esperado:
            // remuneracionBrutaAnual = 4000 * 12 = 48000
            // deduccion = 7 * 5000 = 35000
            // baseImponible = 13000 → 13000 / 5000 = 2.6 UIT
            // Impuesto anual = 2.6 UIT * 5000 * 0.08 = 1040
            // Mensual = 1040 / 12 = 86.67
            decimal esperado = 86.67m;

            // Assert
            Assert.AreEqual(esperado, detalle.ImpuestoRentaMensual);
        }

        [TestMethod]
        public void CalcularImpuestoRentaQuinta_VariosTramos_DevuelveCorrecto()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 8500m // Base imponible que cruza tramos 1 y 2
            };

            var tramos = new List<ImpuestoRentaTramo>
        {
            new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
            new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 },
            new ImpuestoRentaTramo { NumeroTramo = 3, LimiteInferiorUIT = 20, LimiteSuperiorUIT = null, TasaPorcentaje = 17 }
        };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Cálculo esperado:
            // remuneracionBrutaAnual = 8500 * 12 = 102000
            // deduccion = 7 * 5000 = 35000
            // baseImponible = 67000 → 67000 / 5000 = 13.4 UIT
            // Tramo1: 0–5 UIT = 5*5000*0.08 = 2000
            // Tramo2: 5–13.4 UIT = 8.4*5000*0.14 = 5880
            // Total anual = 2000 + 5880 = 7880 → mensual = 7880/12 = 656.67
            decimal esperadoMensual = 656.67m;

            // Assert
            Assert.AreEqual(esperadoMensual, detalle.ImpuestoRentaMensual);
        }

        [TestMethod]
        public void CalcularImpuestoRentaQuinta_TodosTramos_DevuelveCorrecto()
        {
            // Arrange
            var detalle = new DetalleNomina
            {
                RemuneracionBruta = 20000m // Base imponible que cruza los 3 tramos
            };

            var tramos = new List<ImpuestoRentaTramo>
        {
            new ImpuestoRentaTramo { NumeroTramo = 1, LimiteInferiorUIT = 0, LimiteSuperiorUIT = 5, TasaPorcentaje = 8 },
            new ImpuestoRentaTramo { NumeroTramo = 2, LimiteInferiorUIT = 5, LimiteSuperiorUIT = 20, TasaPorcentaje = 14 },
            new ImpuestoRentaTramo { NumeroTramo = 3, LimiteInferiorUIT = 20, LimiteSuperiorUIT = null, TasaPorcentaje = 17 }
        };

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Cálculo esperado:
            // remuneracionBrutaAnual = 20000*12 = 240000
            // deduccion = 7*5000 = 35000
            // baseImponible = 205000 → 41 UIT
            // Tramo1: 0-5 UIT = 5*5000*0.08 = 2000
            // Tramo2: 5-20 UIT = 15*5000*0.14 = 10500
            // Tramo3: 20-41 UIT = 21*5000*0.17 = 17850
            // Total anual = 2000+10500+17850=30350 → mensual=30350/12=2529.17
            decimal esperadoMensual = 2529.17m;

            // Assert
            Assert.AreEqual(esperadoMensual, detalle.ImpuestoRentaMensual);
        }




        // -------------------------------------------------------------
        // PRUEBA 7: Calcular Totales (Ingresos, Descuentos y Neto)
        // -------------------------------------------------------------
        [TestMethod]
        public void CalcularTotales_DevuelveCorrectos()
        {
            var detalle = new DetalleNomina
            {
                SueldoBasico = 3000m,
                AsignacionFamiliar = 100m,
                BonosRegulares = 200m,
                OtrosIngresos = 50m,
                DescuentoAFP = 300m,
                DescuentoFaltas = 100m,
                ImpuestoRentaMensual = 100m
            };

            detalle.CalcularTotales();

            decimal ingresosEsperados = 3350m;
            decimal descuentosEsperados = 500m;
            decimal netoEsperado = 2850m;

            Assert.AreEqual(ingresosEsperados, detalle.TotalIngresos, "Total de ingresos incorrecto");
            Assert.AreEqual(descuentosEsperados, detalle.TotalDescuentos, "Total de descuentos incorrecto");
            Assert.AreEqual(netoEsperado, detalle.NetoPagar, "Neto a pagar incorrecto");
        }

        // -------------------------------------------------------------
        // PRUEBA 8: Validar Excepción si contrato es nulo
        // -------------------------------------------------------------
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalcularPagoTotalHorasExtras_SinContrato_LanzaExcepcion()
        {
            var detalle = new DetalleNomina();
            detalle.CalcularPagoTotalHorasExtras();
        }
    }
}
