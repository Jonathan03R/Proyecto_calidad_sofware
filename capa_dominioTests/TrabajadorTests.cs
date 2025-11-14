using Microsoft.VisualStudio.TestTools.UnitTesting;
using capa_dominio;
using System;
using System.Collections.Generic;

namespace capa_pruebas
{
    [TestClass]
    public class TrabajadorTests
    {
    
        // TEST 1: Asignación de propiedades básicas

        [TestMethod]
        public void Trabajador_AsignaPropiedadesBasicas()
        {
            // Arrange & Act
            var trabajador = new Trabajador
            {
                TrabajadorId = 1,
                Codigo = "TRA_001",
                Nombres = "Juan Carlos",
                Apellidos = "Pérez García",
                TipoIdentificacion = "DNI",
                Identificacion = "12345678",
                Estado = 'A'
            };

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"TRABAJADOR -> Id:{trabajador.TrabajadorId} | Codigo:{trabajador.Codigo}");
            Console.WriteLine($"Nombres:{trabajador.Nombres} | Apellidos:{trabajador.Apellidos}");
            Console.WriteLine($"Identificacion:{trabajador.TipoIdentificacion}-{trabajador.Identificacion}");
            Console.WriteLine($"Estado:{trabajador.Estado}");

            // Assert
            Assert.AreEqual(1, trabajador.TrabajadorId);
            Assert.AreEqual("TRA_001", trabajador.Codigo);
            Assert.AreEqual("Juan Carlos", trabajador.Nombres);
            Assert.AreEqual("Pérez García", trabajador.Apellidos);
            Assert.AreEqual('A', trabajador.Estado);
        }

        // TEST 2: Estado activo

        [TestMethod]
        public void Trabajador_ValidaEstadoActivo()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                Estado = 'A'
            };

            // Act
            bool esActivo = trabajador.Estado == 'A';

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ESTADO ACTIVO -> Estado:{trabajador.Estado} | EsActivo:{esActivo}");

            // Assert
            Assert.IsTrue(esActivo);
        }

        // TEST 3: Estado inactivo

        [TestMethod]
        public void Trabajador_ValidaEstadoInactivo()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                Estado = 'I'
            };

            // Act
            bool esInactivo = trabajador.Estado == 'I';

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ESTADO INACTIVO -> Estado:{trabajador.Estado} | EsInactivo:{esInactivo}");

            // Assert
            Assert.IsTrue(esInactivo);
        }

        // TEST 4: Asignación familiar SIN hijos

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_SinHijos_RetornaFalse()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 1,
                Hijos = new List<Hijo>() // Lista vacía
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR SIN HIJOS -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"CantidadHijos:{trabajador.Hijos.Count} | TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsFalse(tieneDerecho);
        }

        // TEST 5: Asignación familiar CON hijo menor de 18

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_ConHijoMenor_RetornaTrue()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 2,
                Hijos = new List<Hijo>
                {
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-10), // Hijo de 10 años
                        Estado = 'a', // activo
                        TieneDiscapacidad = false,
                        Estudia = false
                    }
                }
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR CON HIJO MENOR -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"CantidadHijos:{trabajador.Hijos.Count}");
            Console.WriteLine($"EdadHijo:{(DateTime.Now - trabajador.Hijos[0].FechaNacimiento).Days / 365} años");
            Console.WriteLine($"TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsTrue(tieneDerecho);
        }

        // TEST 6: Asignación familiar CON hijo mayor con discapacidad

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_HijoMayorConDiscapacidad_RetornaTrue()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 3,
                Hijos = new List<Hijo>
                {
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-25), // Hijo de 25 años
                        Estado = 'a',
                        TieneDiscapacidad = true, // ✅ Con discapacidad
                        Estudia = false
                    }
                }
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR HIJO DISCAPACIDAD -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"EdadHijo:{(DateTime.Now - trabajador.Hijos[0].FechaNacimiento).Days / 365} años");
            Console.WriteLine($"TieneDiscapacidad:{trabajador.Hijos[0].TieneDiscapacidad}");
            Console.WriteLine($"TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsTrue(tieneDerecho);
        }

  
        // TEST 7: Asignación familiar CON hijo mayor estudiante

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_HijoMayorEstudiante_RetornaTrue()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 4,
                Hijos = new List<Hijo>
                {
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-20), // Hijo de 20 años
                        Estado = 'a',
                        TieneDiscapacidad = false,
                        Estudia = true // ✅ Estudiante
                    }
                }
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR HIJO ESTUDIANTE -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"EdadHijo:{(DateTime.Now - trabajador.Hijos[0].FechaNacimiento).Days / 365} años");
            Console.WriteLine($"Estudia:{trabajador.Hijos[0].Estudia}");
            Console.WriteLine($"TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsTrue(tieneDerecho);
        }


        // TEST 8: Asignación familiar CON hijo inactivo

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_HijoInactivo_RetornaFalse()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 5,
                Hijos = new List<Hijo>
                {
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-10),
                        Estado = 'i', // ❌ inactivo
                        TieneDiscapacidad = false,
                        Estudia = false
                    }
                }
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR HIJO INACTIVO -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"EstadoHijo:{trabajador.Hijos[0].Estado}");
            Console.WriteLine($"TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsFalse(tieneDerecho, "No debe tener derecho si el hijo está inactivo");
        }


        // TEST 9: Asignación familiar CON múltiples hijos

        [TestMethod]
        public void TieneDerechoAsignacionFamiliar_VariosHijos_RetornaTrue()
        {
            // Arrange
            var trabajador = new Trabajador
            {
                TrabajadorId = 6,
                Hijos = new List<Hijo>
                {
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-25), // Mayor sin derecho
                        Estado = 'a',
                        TieneDiscapacidad = false,
                        Estudia = false
                    },
                    new Hijo
                    {
                        FechaNacimiento = DateTime.Now.AddYears(-15), // ✅ Menor de 18
                        Estado = 'a',
                        TieneDiscapacidad = false,
                        Estudia = false
                    }
                }
            };

            // Act
            bool tieneDerecho = trabajador.TieneDerechoAsignacionFamiliar();

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"ASIGNACION FAMILIAR VARIOS HIJOS -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"CantidadHijos:{trabajador.Hijos.Count}");
            Console.WriteLine($"Hijo1_Edad:{(DateTime.Now - trabajador.Hijos[0].FechaNacimiento).Days / 365} años");
            Console.WriteLine($"Hijo2_Edad:{(DateTime.Now - trabajador.Hijos[1].FechaNacimiento).Days / 365} años");
            Console.WriteLine($"TieneDerecho:{tieneDerecho}");

            // Assert
            Assert.IsTrue(tieneDerecho, "Debe tener derecho si al menos uno de los hijos cumple los requisitos");
        }


        // TEST 10: Listas de contactos y horas trabajadas

        [TestMethod]
        public void Trabajador_AsignaListasCorrectamente()
        {
            // Arrange & Act
            var trabajador = new Trabajador
            {
                Contactos = new List<Contacto>
                {
                    new Contacto { /* propiedades del contacto */ }
                },
                HorasTrabajadas = new List<HoraTrabajada>
                {
                    new HoraTrabajada { /* propiedades */ }
                }
            };

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"LISTAS -> Contactos:{trabajador.Contactos.Count} | HorasTrabajadas:{trabajador.HorasTrabajadas.Count}");

            // Assert
            Assert.AreEqual(1, trabajador.Contactos.Count);
            Assert.AreEqual(1, trabajador.HorasTrabajadas.Count);
        }

        // =======================================================
        // TEST 11: Asignación de contrato
        // =======================================================
        [TestMethod]
        public void Trabajador_AsignaContratoCorrectamente()
        {
            // Arrange & Act
            var trabajador = new Trabajador
            {
                TrabajadorId = 7,
                Contrato = new Contrato
                {
                    ContratoId = 100,
                    ContratoSalario = 3000
                }
            };

            // Seguimiento
            Console.WriteLine();
            Console.WriteLine("Seguimiento de depuración:");
            Console.WriteLine($"CONTRATO -> TrabajadorId:{trabajador.TrabajadorId}");
            Console.WriteLine($"ContratoId:{trabajador.Contrato.ContratoId} | Salario:{trabajador.Contrato.ContratoSalario:C}");

            // Assert
            Assert.IsNotNull(trabajador.Contrato);
            Assert.AreEqual(100, trabajador.Contrato.ContratoId);
            Assert.AreEqual(3000, trabajador.Contrato.ContratoSalario);
        }
    }
}