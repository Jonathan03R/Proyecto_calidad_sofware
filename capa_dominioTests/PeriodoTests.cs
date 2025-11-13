using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.Tests
{
    [TestClass]
    public class PeriodoTests
    {

        [TestMethod]
        public void EstaProcesado_CuandoEstadoEsProcesado_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoEstado = "Procesado"
            };

            // Act
            bool resultado = periodo.EstaProcesado();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: resultado = true
        }

        [TestMethod]
        public void EstaProcesado_CuandoEstadoEsActivo_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoEstado = "Activo"
            };

            // Act
            bool resultado = periodo.EstaProcesado();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: resultado = false
        }

        [TestMethod]
        public void EsActivo_CuandoEstadoEsActivo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoEstado = "Activo"
            };

            // Act
            bool resultado = periodo.EsActivo();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: resultado = true
        }

        [TestMethod]
        public void EsActivo_CuandoEstadoEsProcesado_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoEstado = "Procesado"
            };

            // Act
            bool resultado = periodo.EsActivo();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: resultado = false
        }

        [TestMethod]
        public void EsPeriodoActual_CuandoFechaActualEstaEnRango_RetornaTrue()
        {
            // Arrange
            DateTime hoy = DateTime.Today;
            var periodo = new Periodo
            {
                PeriodoFechaInicio = hoy.AddDays(-5),
                PeriodoFechaFin = hoy.AddDays(5)
            };

            // Act
            bool resultado = periodo.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: resultado = true (hoy está entre inicio y fin)
        }

        [TestMethod]
        public void EsPeriodoActual_CuandoFechaActualEsIgualAInicio_RetornaTrue()
        {
            // Arrange
            DateTime hoy = DateTime.Today;
            var periodo = new Periodo
            {
                PeriodoFechaInicio = hoy,
                PeriodoFechaFin = hoy.AddDays(10)
            };

            // Act
            bool resultado = periodo.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: resultado = true (hoy = fecha inicio)
        }

        [TestMethod]
        public void EsPeriodoActual_CuandoFechaActualEsAnterior_RetornaFalse()
        {
            // Arrange
            DateTime hoy = DateTime.Today;
            var periodo = new Periodo
            {
                PeriodoFechaInicio = hoy.AddDays(1),
                PeriodoFechaFin = hoy.AddDays(10)
            };

            // Act
            bool resultado = periodo.EsPeriodoActual();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: resultado = false (hoy es antes del inicio)
        }

        [TestMethod]
        public void EsPeriodoActual_CuandoFechaActualEsPosterior_RetornaFalse()
        {
            // Arrange
            DateTime hoy = DateTime.Today;
            var periodo = new Periodo
            {
                PeriodoFechaInicio = hoy.AddDays(-10),
                PeriodoFechaFin = hoy.AddDays(-1)
            };

            // Act
            bool resultado = periodo.EsPeriodoActual();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: resultado = false (hoy es después del fin)
        }
    }
}