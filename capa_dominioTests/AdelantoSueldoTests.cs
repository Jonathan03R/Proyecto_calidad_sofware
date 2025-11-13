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
    public class AdelantoSueldoTests
    {

        [TestMethod]
        public void EsMontoValido_MontoPositivo_RetornaTrue()
        {
            // Arrange
            var adelanto = new AdelantoSueldo
            {
                AdelantoMonto = 500.00m
            };

            // Act
            bool resultado = adelanto.EsMontoValido();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (monto 500.00 es mayor que 0)
        }

        [TestMethod]
        public void EsMontoValido_MontoCero_RetornaFalse()
        {
            // Arrange
            var adelanto = new AdelantoSueldo
            {
                AdelantoMonto = 0m
            };

            // Act
            bool resultado = adelanto.EsMontoValido();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false (monto 0 no es mayor que 0)
        }

        [TestMethod]
        public void EsMontoValido_MontoNegativo_RetornaFalse()
        {
            // Arrange
            var adelanto = new AdelantoSueldo
            {
                AdelantoMonto = -100.00m
            };

            // Act
            bool resultado = adelanto.EsMontoValido();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false (monto -100.00 no es mayor que 0)
        }

        [TestMethod]
        public void EsMontoValido_MontoMuyPequeno_RetornaTrue()
        {
            // Arrange
            var adelanto = new AdelantoSueldo
            {
                AdelantoMonto = 0.01m
            };

            // Act
            bool resultado = adelanto.EsMontoValido();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (monto 0.01 es mayor que 0)
        }

        [TestMethod]
        public void EsMontoValido_MontoGrande_RetornaTrue()
        {
            // Arrange
            var adelanto = new AdelantoSueldo
            {
                AdelantoMonto = 1000.00m
            };

            // Act
            bool resultado = adelanto.EsMontoValido();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (monto 1000.00 es mayor que 0)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaDentroDelPeriodo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 11, 15)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (15/11/2025 está entre 01/11/2025 y 30/11/2025)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaEnInicioDelPeriodo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 11, 1)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (01/11/2025 es igual a la fecha de inicio)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaEnFinDelPeriodo_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 11, 30)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (30/11/2025 es igual a la fecha de fin)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaAntesDelPeriodo_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 10, 31)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false (31/10/2025 es anterior al 01/11/2025)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaDespuesDelPeriodo_RetornaFalse()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 12, 1)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsFalse(resultado);
            // Resultado esperado: false (01/12/2025 es posterior al 30/11/2025)
        }

        [TestMethod]
        public void EsPeriodoActual_FechaConHora_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 1),
                PeriodoFechaFin = new DateTime(2025, 11, 30)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 11, 15, 14, 30, 0)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (15/11/2025 14:30:00 está dentro del periodo)
        }

        [TestMethod]
        public void EsPeriodoActual_PeriodoUnDia_FechaCorrecta_RetornaTrue()
        {
            // Arrange
            var periodo = new Periodo
            {
                PeriodoFechaInicio = new DateTime(2025, 11, 15),
                PeriodoFechaFin = new DateTime(2025, 11, 15)
            };

            var adelanto = new AdelantoSueldo
            {
                Periodo = periodo,
                AdelantoFecha = new DateTime(2025, 11, 15)
            };

            // Act
            bool resultado = adelanto.EsPeriodoActual();

            // Assert
            Assert.IsTrue(resultado);
            // Resultado esperado: true (periodo de un solo día y fecha coincide)
        }

        [TestMethod]
        public void Propiedades_AsignarYObtenerValores_FuncionaCorrectamente()
        {
            // Arrange
            var trabajador = new Trabajador { TrabajadorId = 1, Nombres = "Juan", Apellidos = "Pérez" };
            var periodo = new Periodo { PeriodoId = 1, PeriodoNombre = "Noviembre 2025" };
            var fecha = new DateTime(2025, 11, 12);

            var adelanto = new AdelantoSueldo();

            // Act
            adelanto.AdelantoId = 100;
            adelanto.Trabajador = trabajador;
            adelanto.Periodo = periodo;
            adelanto.AdelantoMonto = 1500.50m;
            adelanto.AdelantoFecha = fecha;
            adelanto.AdelantoMotivo = "Emergencia médica";
            adelanto.AdelantoObservaciones = "Aprobado por gerencia";

            // Assert
            Assert.AreEqual(100, adelanto.AdelantoId);
            Assert.AreEqual(trabajador, adelanto.Trabajador);
            Assert.AreEqual(periodo, adelanto.Periodo);
            Assert.AreEqual(1500.50m, adelanto.AdelantoMonto);
            Assert.AreEqual(fecha, adelanto.AdelantoFecha);
            Assert.AreEqual("Emergencia médica", adelanto.AdelantoMotivo);
            Assert.AreEqual("Aprobado por gerencia", adelanto.AdelantoObservaciones);
            // Resultado esperado: Todas las propiedades mantienen los valores asignados
        }
    }
}