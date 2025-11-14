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

    }
}