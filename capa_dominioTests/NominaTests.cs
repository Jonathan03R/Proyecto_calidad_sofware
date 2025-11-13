using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace capa_dominio.Tests
{
    [TestClass()]
    public class NominaTests
    {

        [TestMethod()]
        public void EstaProcesando_CuandoEstadoEsProcesando_RetornaTrue()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Procesando"
            };

            // Act
            bool resultado = nomina.EstaProcesando();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque el estado es "Procesando"
        }

        [TestMethod()]
        public void EstaProcesando_CuandoEstadoEsExitoso_RetornaFalse()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Exitoso"
            };

            // Act
            bool resultado = nomina.EstaProcesando();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque el estado no es "Procesando"
        }

        [TestMethod()]
        public void EsExitosa_CuandoEstadoEsExitoso_RetornaTrue()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Exitoso"
            };

            // Act
            bool resultado = nomina.EsExitosa();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque el estado es "Exitoso"
        }

        [TestMethod()]
        public void EsExitosa_CuandoEstadoEsProcesando_RetornaFalse()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Procesando"
            };

            // Act
            bool resultado = nomina.EsExitosa();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque el estado no es "Exitoso"
        }

        [TestMethod()]
        public void TieneErrores_CuandoEstadoEsConErrores_RetornaTrue()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Con Errores"
            };

            // Act
            bool resultado = nomina.TieneErrores();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque el estado es "Con Errores"
        }

        [TestMethod()]
        public void TieneErrores_CuandoEstadoEsExitoso_RetornaFalse()
        {
            // Arrange
            var nomina = new Nomina
            {
                NominaEstado = "Exitoso"
            };

            // Act
            bool resultado = nomina.TieneErrores();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque el estado no es "Con Errores"
        }

        [TestMethod()]
        public void CalcularTotales_ConDetallesValidos_CalculaCorrectamente()
        {
            // Arrange
            var nomina = new Nomina
            {
                Detalles = new List<DetalleNomina>
                {
                    new DetalleNomina
                    {
                        TotalIngresos = 3000m,
                        TotalDescuentos = 500m,
                        RemuneracionBruta = 2800m,
                        Contrato = new Contrato { Trabajador = new Trabajador { TrabajadorId = 1 } }
                    },
                    new DetalleNomina
                    {
                        TotalIngresos = 4000m,
                        TotalDescuentos = 700m,
                        RemuneracionBruta = 3700m,
                        Contrato = new Contrato { Trabajador = new Trabajador { TrabajadorId = 2 } }
                    },
                    new DetalleNomina
                    {
                        TotalIngresos = 2500m,
                        TotalDescuentos = 400m,
                        RemuneracionBruta = 2300m,
                        Contrato = new Contrato { Trabajador = new Trabajador { TrabajadorId = 3 } }
                    }
                }
            };

            // Act
            nomina.CalcularTotales();

            // Assert
            Assert.AreEqual(3, nomina.NominaTotalEmpleados);
            Assert.AreEqual(9500m, nomina.NominaTotalBruto); // 3000 + 4000 + 2500
            Assert.AreEqual(1600m, nomina.NominaTotalDescuentos); // 500 + 700 + 400
            Assert.AreEqual(8800m, nomina.NominaTotalNeto); // 2800 + 3700 + 2300

            // Resultado esperado:
            // - Total empleados: 3
            // - Total bruto: 9500
            // - Total descuentos: 1600
            // - Total neto: 8800
        }

        [TestMethod()]
        public void CalcularTotales_ConUnSoloDetalle_CalculaCorrectamente()
        {
            // Arrange
            var nomina = new Nomina
            {
                Detalles = new List<DetalleNomina>
                {
                    new DetalleNomina
                    {
                        TotalIngresos = 5000m,
                        TotalDescuentos = 800m,
                        RemuneracionBruta = 4500m,
                        Contrato = new Contrato { Trabajador = new Trabajador { TrabajadorId = 1 } }
                    }
                }
            };

            // Act
            nomina.CalcularTotales();

            // Assert
            Assert.AreEqual(1, nomina.NominaTotalEmpleados);
            Assert.AreEqual(5000m, nomina.NominaTotalBruto);
            Assert.AreEqual(800m, nomina.NominaTotalDescuentos);
            Assert.AreEqual(4500m, nomina.NominaTotalNeto);

            // Resultado esperado:
            // - Total empleados: 1
            // - Total bruto: 5000
            // - Total descuentos: 800
            // - Total neto: 4500
        }

        [TestMethod()]
        public void CalcularTotales_ConListaVacia_NoCalculaNada()
        {
            // Arrange
            var nomina = new Nomina
            {
                Detalles = new List<DetalleNomina>()
            };

            // Act
            nomina.CalcularTotales();

            // Assert
            Assert.AreEqual(0, nomina.NominaTotalEmpleados);
            Assert.AreEqual(0m, nomina.NominaTotalBruto);
            Assert.AreEqual(0m, nomina.NominaTotalDescuentos);
            Assert.AreEqual(0m, nomina.NominaTotalNeto);

            // Resultado esperado: todos los valores en 0 porque no hay detalles
        }

        [TestMethod()]
        public void CalcularTotales_ConDetallesNulos_NoCalculaNada()
        {
            // Arrange
            var nomina = new Nomina
            {
                Detalles = null
            };

            // Act
            nomina.CalcularTotales();

            // Assert
            Assert.AreEqual(0, nomina.NominaTotalEmpleados);
            Assert.AreEqual(0m, nomina.NominaTotalBruto);
            Assert.AreEqual(0m, nomina.NominaTotalDescuentos);
            Assert.AreEqual(0m, nomina.NominaTotalNeto);

            // Resultado esperado: todos los valores en 0 porque Detalles es null
        }

        [TestMethod()]
        public void CalcularTotales_ConValoresCero_CalculaCorrectamente()
        {
            // Arrange
            var nomina = new Nomina
            {
                Detalles = new List<DetalleNomina>
                {
                    new DetalleNomina
                    {
                        TotalIngresos = 0m,
                        TotalDescuentos = 0m,
                        RemuneracionBruta = 0m,
                        Contrato = new Contrato { Trabajador = new Trabajador { TrabajadorId = 1 } }
                    }
                }
            };

            // Act
            nomina.CalcularTotales();

            // Assert
            Assert.AreEqual(1, nomina.NominaTotalEmpleados);
            Assert.AreEqual(0m, nomina.NominaTotalBruto);
            Assert.AreEqual(0m, nomina.NominaTotalDescuentos);
            Assert.AreEqual(0m, nomina.NominaTotalNeto);

            // Resultado esperado:
            // - Total empleados: 1 (cuenta el detalle aunque tenga valores en 0)
            // - Todos los totales: 0
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConNominaExitosaEnPeriodo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo { PeriodoId = 1 };
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = periodo,
                    NominaEstado = "Exitoso"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 1);

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque existe una nómina exitosa en el período 1
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConNominaProcesandoEnPeriodo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo { PeriodoId = 2 };
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = periodo,
                    NominaEstado = "Procesando"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 2);

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque existe una nómina procesando en el período 2
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConNominaConErroresEnPeriodo_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo { PeriodoId = 3 };
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = periodo,
                    NominaEstado = "Con Errores"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 3);

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque la nómina tiene errores (no cuenta)
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConPeriodoDiferente_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo { PeriodoId = 1 };
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = periodo,
                    NominaEstado = "Exitoso"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 2);

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque no hay nóminas en el período 2
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConListaVacia_RetornaFalse()
        {
            // Arrange
            var nominas = new List<Nomina>();

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 1);

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque la lista está vacía
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConListaNula_RetornaFalse()
        {
            // Arrange
            List<Nomina> nominas = null;

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 1);

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque la lista es null
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConVariasNominasYUnaSolaExitosaEnPeriodo_RetornaTrue()
        {
            // Arrange
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = new Periodo { PeriodoId = 1 },
                    NominaEstado = "Con Errores"
                },
                new Nomina
                {
                    Periodo = new Periodo { PeriodoId = 2 },
                    NominaEstado = "Exitoso"
                },
                new Nomina
                {
                    Periodo = new Periodo { PeriodoId = 3 },
                    NominaEstado = "Procesando"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 2);

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true porque existe una nómina exitosa en el período 2
        }

        [TestMethod()]
        public void ExisteEnPeriodo_ConPeriodoNuloEnNomina_RetornaFalse()
        {
            // Arrange
            var nominas = new List<Nomina>
            {
                new Nomina
                {
                    Periodo = null,
                    NominaEstado = "Exitoso"
                }
            };

            // Act
            bool resultado = Nomina.ExisteEnPeriodo(nominas, 1);

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false porque el período de la nómina es null
        }
    }
}