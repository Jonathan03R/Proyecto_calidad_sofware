using System;
using System.Collections.Generic;
using capa_aplicacion.Servicios;
using capa_dominio.dto;

namespace capa_presentacion.Pruebas
{
    public class PruebaServicioContratos
    {
        public void EjecutarPrueba()
        {
            try
            {
                var servicio = new ServicioContratos();

                Console.WriteLine("=== 🔹 Prueba de Contratos 🔹 ===\n");

                // 1️⃣ Crear un contrato de prueba
                var nuevoContrato = new ContratoDTO
                {
                    TrabajadorId = 1,  // Asegúrate que exista
                    CargoId = 1,
                    AreaId = 1,
                    TipoPensionId = 1,
                    TipoSalarioId = 1,
                    TipoJornadaId = 1,
                    FechaInicio = DateTime.Now,
                    Salario = 4500,
                    ModoPago = "Transferencia bancaria",
                    DescripcionFunciones = "Prueba de creación de contrato",
                    Observaciones = "Contrato generado desde prueba lógica"
                };

                int idGenerado = servicio.CrearContrato(nuevoContrato);
                Console.WriteLine($"✅ Contrato creado con ID: {idGenerado}\n");

                // 2️⃣ Consultar contratos del trabajador
                var listaContratos = servicio.ConsultarContratosPorTrabajador(1);
                Console.WriteLine($"📋 Total contratos del trabajador 1: {listaContratos.Count}\n");

                // 3️⃣ Finalizar el último contrato de prueba
                if (listaContratos.Count > 0)
                {
                    var ultimoContrato = listaContratos[listaContratos.Count - 1];
                    if (ultimoContrato.ContratoId.HasValue)
                    {
                        var resultado = servicio.FinalizarContrato(ultimoContrato.ContratoId.Value, "Contrato finalizado desde prueba");
                        Console.WriteLine($"🔹 Contrato actualizado: {resultado.contratoActualizado}, cambio registrado: {resultado.cambioRegistrado}\n");
                    }
                }

                // 4️⃣ Listas de referencia
                Console.WriteLine("=== 🔹 Listas de referencia 🔹 ===\n");

                // Trabajadores
                Console.WriteLine("=== Trabajadores ===");
                List<TrabajadorDTO> trabajadores = servicio.ObtenerTrabajadores();
                foreach (var t in trabajadores)
                    Console.WriteLine($"ID: {t.TrabajadorId}, Nombre: {t.NombreCompleto}");
                Console.WriteLine();

                // Áreas
                Console.WriteLine("=== Áreas ===");
                List<AreaDTO> areas = servicio.ObtenerAreas();
                foreach (var a in areas)
                    Console.WriteLine($"ID: {a.AreaId}, Nombre: {a.NombreArea}");
                Console.WriteLine();

                // Cargos
                Console.WriteLine("=== Cargos ===");
                List<CargoDTO> cargos = servicio.ObtenerCargos();
                foreach (var c in cargos)
                    Console.WriteLine($"ID: {c.CargoId}, Nombre: {c.NombreCargo}");
                Console.WriteLine();

                // Estados de contrato
                Console.WriteLine("=== Estados de contrato ===");
                List<EstadoContratoDTO> estados = servicio.ObtenerEstadosContrato();
                foreach (var e in estados)
                    Console.WriteLine($"ID: {e.EstadoId}, Estado: {e.NombreEstado}");
                Console.WriteLine();

                // Tipos de pensión
                Console.WriteLine("=== Tipos de pensión ===");
                List<TipoPensionDTO> pensiones = servicio.ObtenerTiposPension();
                foreach (var p in pensiones)
                    Console.WriteLine($"ID: {p.TipoPensionId}, Tipo: {p.NombreTipo}");
                Console.WriteLine();

                Console.WriteLine("=== ✅ Prueba completa finalizada ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error durante la prueba: {ex.Message}");
            }
        }
    }
}
