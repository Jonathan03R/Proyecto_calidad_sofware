using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;

namespace capa_pruebas
{
    [TestClass]
    public class TipoJornadaTests
    {
        [TestMethod]
        public void TipoJornada_AsignaPropiedades()
        {
            // Arrange & Act
            var tipoJornada = new TipoJornada
            {
                TipoJornadaId = 1,
                TipoJornadaNombre = "Tiempo Completo",
                TipoJornadaDescripcion = "Jornada de 48 horas semanales",
                TipoJornadaEstado = 'A',
                TipoJornadaFechaCreacion = DateTime.Now
            };

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"TIPO JORNADA -> Id:{tipoJornada.TipoJornadaId} | Nombre:{tipoJornada.TipoJornadaNombre}");
            Console.WriteLine($"Descripcion:{tipoJornada.TipoJornadaDescripcion}");
            Console.WriteLine($"Estado:{tipoJornada.TipoJornadaEstado} | FechaCreacion:{tipoJornada.TipoJornadaFechaCreacion:yyyy-MM-dd}");

            // Assert
            Assert.AreEqual(1, tipoJornada.TipoJornadaId);
            Assert.AreEqual("Tiempo Completo", tipoJornada.TipoJornadaNombre);
        }

        [TestMethod]
        public void TipoJornada_EsTiempoCompleto_RetornaTrue()
        {
            // Arrange
            var tipoJornada = new TipoJornada
            {
                TipoJornadaNombre = "Tiempo Completo"
            };

            // Act
            bool esTiempoCompleto = tipoJornada.EsTiempoCompleto();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"TIEMPO COMPLETO -> Nombre:{tipoJornada.TipoJornadaNombre} | EsTiempoCompleto:{esTiempoCompleto}");

            // Assert
            Assert.IsTrue(esTiempoCompleto);
        }

        [TestMethod]
        public void TipoJornada_EsMedioTiempo_RetornaTrue()
        {
            // Arrange
            var tipoJornada = new TipoJornada
            {
                TipoJornadaNombre = "Medio Tiempo"
            };

            // Act
            bool esMedioTiempo = tipoJornada.EsMedioTiempo();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"MEDIO TIEMPO -> Nombre:{tipoJornada.TipoJornadaNombre} | EsMedioTiempo:{esMedioTiempo}");

            // Assert
            Assert.IsTrue(esMedioTiempo);
        }

        [TestMethod]
        public void TipoJornada_EsPorHoras_RetornaTrue()
        {
            // Arrange
            var tipoJornada = new TipoJornada
            {
                TipoJornadaNombre = "Por Horas"
            };

            // Act
            bool esPorHoras = tipoJornada.EsPorHoras();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"POR HORAS -> Nombre:{tipoJornada.TipoJornadaNombre} | EsPorHoras:{esPorHoras}");

            // Assert
            Assert.IsTrue(esPorHoras);
        }

        [TestMethod]
        public void TipoJornada_ValidaEstadoActivo()
        {
            // Arrange
            var tipoJornada = new TipoJornada
            {
                TipoJornadaEstado = 'A'
            };

            // Act
            bool esActivo = tipoJornada.TipoJornadaEstado == 'A';

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"VALIDACION ESTADO -> Estado:{tipoJornada.TipoJornadaEstado} | EsActivo:{esActivo}");

            // Assert
            Assert.IsTrue(esActivo);
        }

        [TestMethod]
        public void TipoJornada_MultiplesValidaciones()
        {
            // Arrange
            var tiempoCompleto = new TipoJornada { TipoJornadaNombre = "Tiempo Completo" };
            var medioTiempo = new TipoJornada { TipoJornadaNombre = "Medio Tiempo" };
            var porHoras = new TipoJornada { TipoJornadaNombre = "Por Horas" };

            // Act & Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"COMPARACION JORNADAS:");
            Console.WriteLine($"  {tiempoCompleto.TipoJornadaNombre} -> EsTiempoCompleto:{tiempoCompleto.EsTiempoCompleto()}");
            Console.WriteLine($"  {medioTiempo.TipoJornadaNombre} -> EsMedioTiempo:{medioTiempo.EsMedioTiempo()}");
            Console.WriteLine($"  {porHoras.TipoJornadaNombre} -> EsPorHoras:{porHoras.EsPorHoras()}");

            // Assert
            Assert.IsTrue(tiempoCompleto.EsTiempoCompleto());
            Assert.IsTrue(medioTiempo.EsMedioTiempo());
            Assert.IsTrue(porHoras.EsPorHoras());
            Assert.IsFalse(tiempoCompleto.EsMedioTiempo());
        }
    }
}
