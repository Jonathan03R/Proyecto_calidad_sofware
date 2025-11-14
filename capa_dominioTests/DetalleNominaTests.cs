using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio.Tests
{
    [TestClass()]
    public class DetalleNominaTests
    {
        private Contrato CrearContratoBase()
        {
            return new Contrato
            {
                ContratoSalario = 3000m,
                ContratoTarifaHora = 12.5m,
                TipoPension = new TipoPension { TipoPensionId = 1, Nombre = "ONP" }
            };
        }

        private DetalleNomina CrearDetalleNominaBase()
        {
            return new DetalleNomina
            {
                Contrato = CrearContratoBase()
            };
        }

        private List<TipoHoraExtra> CrearTiposHorasExtras()
        {
            return new List<TipoHoraExtra>
            {
                new TipoHoraExtra
                {
                    TiposHorasExtrasCodigo = "PRIMERAS2",
                    TiposHorasExtrasMultiplicador = 1.25m,
                    TiposHorasExtrasEstado = 'A'
                },
                new TipoHoraExtra
                {
                    TiposHorasExtrasCodigo = "ADICIONALES",
                    TiposHorasExtrasMultiplicador = 1.35m,
                    TiposHorasExtrasEstado = 'A'
                },
                new TipoHoraExtra
                {
                    TiposHorasExtrasCodigo = "SABADO",
                    TiposHorasExtrasMultiplicador = 1.5m,
                    TiposHorasExtrasEstado = 'A'
                },
                new TipoHoraExtra
                {
                    TiposHorasExtrasCodigo = "DOMINGO",
                    TiposHorasExtrasMultiplicador = 2.0m,
                    TiposHorasExtrasEstado = 'A'
                }
            };
        }

        private List<ImpuestoRentaTramo> CrearTramosImpuestoRenta()
        {
            return new List<ImpuestoRentaTramo>
            {
                new ImpuestoRentaTramo
                {
                    NumeroTramo = 1,
                    LimiteInferiorUIT = 0,
                    LimiteSuperiorUIT = 5,
                    TasaPorcentaje = 8m
                },
                new ImpuestoRentaTramo
                {
                    NumeroTramo = 2,
                    LimiteInferiorUIT = 5,
                    LimiteSuperiorUIT = 20,
                    TasaPorcentaje = 14m
                },
                new ImpuestoRentaTramo
                {
                    NumeroTramo = 3,
                    LimiteInferiorUIT = 20,
                    LimiteSuperiorUIT = 35,
                    TasaPorcentaje = 17m
                },
                new ImpuestoRentaTramo
                {
                    NumeroTramo = 4,
                    LimiteInferiorUIT = 35,
                    LimiteSuperiorUIT = 45,
                    TasaPorcentaje = 20m
                },
                new ImpuestoRentaTramo
                {
                    NumeroTramo = 5,
                    LimiteInferiorUIT = 45,
                    LimiteSuperiorUIT = null,
                    TasaPorcentaje = 30m
                }
            };
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_ContratoNulo_LanzaExcepcion()
        {
            // Arrange
            var detalle = new DetalleNomina();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => detalle.CalcularPagoTotalHorasExtras());
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_SinHorasTrabajadas_RetornaCero()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();

            // Act
            detalle.CalcularPagoTotalHorasExtras();

            // Assert
            Assert.AreEqual(0m, detalle.HorasExtras);
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_DiaLunes_Primeras2Horas()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            var tiposHorasExtras = CrearTiposHorasExtras();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 10), // Lunes
                HorasNormales = 8m,
                HorasExtras = 2m,
                TiposHorasExtras = tiposHorasExtras,
                Contrato = detalle.Contrato
            };

            // Act
            horaTrabajada.CalcularPagoDia();

            // Assert - Primeras 2 horas extras: 2 × 12.5 × 1.25 = 31.25
            decimal pagoHorasNormales = 8m * 12.5m; // 100
            decimal pagoHorasExtras = 2m * 12.5m * 1.25m; // 31.25
            decimal totalEsperado = pagoHorasNormales + pagoHorasExtras; // 131.25
            Assert.AreEqual(131.25m, horaTrabajada.TotalDiaTrabajado);
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_DiaLunes_MasDe2Horas()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            var tiposHorasExtras = CrearTiposHorasExtras();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 10), // Lunes
                HorasNormales = 8m,
                HorasExtras = 4m,
                TiposHorasExtras = tiposHorasExtras,
                Contrato = detalle.Contrato
            };

            // Act
            horaTrabajada.CalcularPagoDia();

            // Assert
            // Horas normales: 8 × 12.5 = 100
            // Primeras 2 horas extras: 2 × 12.5 × 1.25 = 31.25
            // Horas adicionales: 2 × 12.5 × 1.35 = 33.75
            // Total: 165
            decimal pagoHorasNormales = 8m * 12.5m;
            decimal pagoPrimeras2 = 2m * 12.5m * 1.25m;
            decimal pagoAdicionales = 2m * 12.5m * 1.35m;
            decimal totalEsperado = pagoHorasNormales + pagoPrimeras2 + pagoAdicionales;
            Assert.AreEqual(165m, horaTrabajada.TotalDiaTrabajado);
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_Sabado_TodasLasHoras()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            var tiposHorasExtras = CrearTiposHorasExtras();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 15), // Sábado
                HorasNormales = 4m,
                HorasExtras = 2m,
                TiposHorasExtras = tiposHorasExtras,
                Contrato = detalle.Contrato
            };

            // Act
            horaTrabajada.CalcularPagoDia();

            // Assert - Sábado: todas las horas × 1.5
            // (4 + 2) × 12.5 × 1.5 = 112.5
            decimal totalEsperado = 6m * 12.5m * 1.5m;
            Assert.AreEqual(112.5m, horaTrabajada.TotalDiaTrabajado);
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_Domingo_TodasLasHoras()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            var tiposHorasExtras = CrearTiposHorasExtras();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 16), // Domingo
                HorasNormales = 4m,
                HorasExtras = 2m,
                TiposHorasExtras = tiposHorasExtras,
                Contrato = detalle.Contrato
            };

            // Act
            horaTrabajada.CalcularPagoDia();

            // Assert - Domingo: todas las horas × 2.0
            // (4 + 2) × 12.5 × 2.0 = 150
            decimal totalEsperado = 6m * 12.5m * 2.0m;
            Assert.AreEqual(150m, horaTrabajada.TotalDiaTrabajado);
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_SinTiposHorasExtras_LanzaExcepcion()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 10),
                HorasNormales = 8m,
                HorasExtras = 2m,
                TiposHorasExtras = null, // Sin tipos de horas extras
                Contrato = detalle.Contrato
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => horaTrabajada.CalcularPagoDia());
        }

        [TestMethod()]
        public void CalcularPagoTotalHorasExtras_SinContrato_LanzaExcepcion()
        {
            // Arrange
            var tiposHorasExtras = CrearTiposHorasExtras();

            var horaTrabajada = new capa_dominio.HoraTrabajada
            {
                Fecha = new DateTime(2025, 11, 10),
                HorasNormales = 8m,
                HorasExtras = 2m,
                TiposHorasExtras = tiposHorasExtras,
                Contrato = null // Sin contrato
            };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => horaTrabajada.CalcularPagoDia());
        }

        [TestMethod()]
        public void CalcularRemuneracionBruta_CalculaCorrectamente()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.Contrato.ContratoSalario = 3000m;
            detalle.AsignacionFamiliar = 300m;
            detalle.HorasExtras = 500m;
            detalle.BonosRegulares = 200m;

            // Act
            detalle.CalcularRemuneracionBruta();

            // Assert
            Assert.AreEqual(4000m, detalle.RemuneracionBruta);
        }

        [TestMethod()]
        public void CalcularRemuneracionBruta_SinBonos_CalculaCorrectamente()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.Contrato.ContratoSalario = 2500m;
            detalle.AsignacionFamiliar = 0m;
            detalle.HorasExtras = 0m;
            detalle.BonosRegulares = 0m;

            // Act
            detalle.CalcularRemuneracionBruta();

            // Assert
            Assert.AreEqual(2500m, detalle.RemuneracionBruta);
        }

        [TestMethod()]
        public void CalculoAsignacionFamiliar_ConDerecho_Calcula10Porciento()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.Contrato.ContratoSalario = 3000m;

            // Act
            var resultado = detalle.CalculoAsignacionFamiliar(true);

            // Assert
            Assert.AreEqual(300m, resultado);
            Assert.AreEqual(300m, detalle.AsignacionFamiliar);
        }

        [TestMethod()]
        public void CalculoAsignacionFamiliar_SinDerecho_RetornaCero()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.Contrato.ContratoSalario = 3000m;

            // Act
            var resultado = detalle.CalculoAsignacionFamiliar(false);

            // Assert
            Assert.AreEqual(0m, resultado);
        }

        [TestMethod()]
        public void CalcularSistemaPensiones_ONP()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4000m;
            detalle.Contrato.TipoPension = new TipoPension { TipoPensionId = 1, Nombre = "ONP" };

            // Act
            detalle.CalcularSistemaPensiones();

            // Assert
            Assert.AreEqual(520m, detalle.AporteONP);
            Assert.AreEqual(0m, detalle.DescuentoAFP);
            Assert.AreEqual("ONP", detalle.SistemasPensionAplicado);
        }

        [TestMethod()]
        public void CalcularSistemaPensiones_AFP()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4500m;
            detalle.Contrato.TipoPension = new TipoPension { TipoPensionId = 2, Nombre = "AFP" };

            // Act
            detalle.CalcularSistemaPensiones();

            // Assert
            Assert.AreEqual(450m, detalle.DescuentoAFP);
            Assert.AreEqual(0m, detalle.AporteONP);
        }

        [TestMethod()]
        public void CalcularSistemaPensiones_SinAfiliacion_RetornaCero()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4000m;
            detalle.Contrato.TipoPension = new TipoPension { TipoPensionId = 6, Nombre = "Sin afiliación" };

            // Act
            detalle.CalcularSistemaPensiones();

            // Assert
            Assert.AreEqual(0m, detalle.AporteONP);
            Assert.AreEqual(0m, detalle.DescuentoAFP);
            Assert.AreEqual("Sin afiliación", detalle.SistemasPensionAplicado);
        }

        [TestMethod()]
        public void CalcularAporteEssalud()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4000m;
            var parametro = new Parametro { ParametroValor = 0.09m };

            // Act
            detalle.CalcularAporteEssalud(parametro);

            // Assert
            Assert.AreEqual(360m, detalle.AporteEssalud);
        }

        [TestMethod()]
        public void CalcularImpuestoRentaQuinta_BaseImponibleNegativa_RetornaCero()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 1000m;
            var tramos = CrearTramosImpuestoRenta();
            decimal valorUIT = 5175m;

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.AreEqual(0m, detalle.ImpuestoRentaMensual);
        }

        [TestMethod()]
        public void CalcularImpuestoRentaQuinta_Tramo1_CalculaCorrectamente()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4000m;
            var tramos = CrearTramosImpuestoRenta();
            decimal valorUIT = 5175m;

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.IsTrue(detalle.ImpuestoRentaMensual > 0);
            Assert.IsTrue(detalle.ImpuestoRentaMensual < 100);
        }

        [TestMethod()]
        public void CalcularImpuestoRentaQuinta_ConListaVacia_RetornaCero()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 8000m;
            var tramos = new List<ImpuestoRentaTramo>();
            decimal valorUIT = 5175m;

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.AreEqual(0m, detalle.ImpuestoRentaMensual);
        }

        [TestMethod()]
        public void CalcularImpuestoRentaQuinta_ConDeduccion7UIT()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 5000m;
            var tramos = CrearTramosImpuestoRenta();
            decimal valorUIT = 5175m;

            // Act
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);

            // Assert
            Assert.IsTrue(detalle.ImpuestoRentaMensual > 0);
        }

        [TestMethod()]
        public void CalcularTotales_CalculaCorrectamente()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 4000m;
            detalle.OtrosIngresos = 500m;
            detalle.AporteONP = 520m;
            detalle.DescuentoAFP = 0m;
            detalle.ImpuestoRentaMensual = 200m;
            detalle.DescuentoAdelantos = 300m;

            // Act
            detalle.CalcularTotales();

            // Assert
            Assert.AreEqual(4500m, detalle.TotalIngresos);
            Assert.AreEqual(1020m, detalle.TotalDescuentos);
            Assert.AreEqual(3480m, detalle.NetoPagar);
        }

        [TestMethod()]
        public void CalcularTotales_SinDescuentos()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.RemuneracionBruta = 3000m;
            detalle.OtrosIngresos = 0m;
            detalle.AporteONP = 0m;
            detalle.DescuentoAFP = 0m;
            detalle.ImpuestoRentaMensual = 0m;
            detalle.DescuentoAdelantos = 0m;

            // Act
            detalle.CalcularTotales();

            // Assert
            Assert.AreEqual(3000m, detalle.TotalIngresos);
            Assert.AreEqual(0m, detalle.TotalDescuentos);
            Assert.AreEqual(3000m, detalle.NetoPagar);
        }

        [TestMethod()]
        public void FlujoCompleto_CalculoNomina_TodosLosMetodos()
        {
            // Arrange
            var detalle = CrearDetalleNominaBase();
            detalle.Contrato.ContratoSalario = 3000m;

            // Act
            detalle.AsignacionFamiliar = detalle.CalculoAsignacionFamiliar(true);
            detalle.CalcularPagoTotalHorasExtras();
            detalle.BonosRegulares = 200m;
            detalle.CalcularRemuneracionBruta();

            var parametroEssalud = new Parametro { ParametroValor = 0.09m };
            detalle.CalcularAporteEssalud(parametroEssalud);
            detalle.CalcularSistemaPensiones();

            var tramos = CrearTramosImpuestoRenta();
            detalle.CalcularImpuestoRentaQuinta(tramos, 5175m);

            detalle.OtrosIngresos = 100m;
            detalle.DescuentoAdelantos = 0m;
            detalle.CalcularTotales();

            // Assert
            Assert.IsTrue(detalle.RemuneracionBruta > 0);
            Assert.IsTrue(detalle.AporteEssalud > 0);
            Assert.IsTrue(detalle.TotalIngresos > 0);
            Assert.IsTrue(detalle.NetoPagar > 0);

            decimal netoPagarEsperado = detalle.TotalIngresos - detalle.TotalDescuentos;
            Assert.AreEqual(netoPagarEsperado, detalle.NetoPagar);

            decimal remuneracionBrutaEsperada = detalle.Contrato.ContratoSalario +
                                                 detalle.AsignacionFamiliar +
                                                 detalle.HorasExtras +
                                                 detalle.BonosRegulares;
            Assert.AreEqual(remuneracionBrutaEsperada, detalle.RemuneracionBruta);
        }
    }
}