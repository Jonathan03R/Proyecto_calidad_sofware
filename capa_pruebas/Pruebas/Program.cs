using System;
using System.Collections.Generic;
using capa_aplicacion.Servicios;
using capa_dominio.dto;

namespace capa_presentacion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Crear instancia del servicio
                ServicioContratos servicio = new ServicioContratos();

                // =================== Trabajadores ===================
                Console.WriteLine("=== Trabajadores ===");
                List<TrabajadorDTO> trabajadores = servicio.ObtenerTrabajadores();
                if (trabajadores.Count == 0)
                    Console.WriteLine("No hay trabajadores registrados.");
                else
                    foreach (var t in trabajadores)
                        Console.WriteLine($"ID: {t.TrabajadorId}, Nombre: {t.NombreCompleto}");

                Console.WriteLine();

                // =================== Áreas ===================
                Console.WriteLine("=== Áreas ===");
                List<AreaDTO> areas = servicio.ObtenerAreas();
                if (areas.Count == 0)
                    Console.WriteLine("No hay áreas registradas.");
                else
                    foreach (var a in areas)
                        Console.WriteLine($"ID: {a.AreaId}, Nombre: {a.NombreArea}");

                Console.WriteLine();

                // =================== Cargos ===================
                Console.WriteLine("=== Cargos ===");
                List<CargoDTO> cargos = servicio.ObtenerCargos();
                if (cargos.Count == 0)
                    Console.WriteLine("No hay cargos registrados.");
                else
                    foreach (var c in cargos)
                        Console.WriteLine($"ID: {c.CargoId}, Nombre: {c.NombreCargo}");

                Console.WriteLine();

                // =================== Estados de contrato ===================
                Console.WriteLine("=== Estados de contrato ===");
                List<EstadoContratoDTO> estados = servicio.ObtenerEstadosContrato();
                if (estados.Count == 0)
                    Console.WriteLine("No hay estados de contrato definidos.");
                else
                    foreach (var e in estados)
                        Console.WriteLine($"ID: {e.EstadoId}, Estado: {e.NombreEstado}");

                Console.WriteLine();

                // =================== Tipos de pensión ===================
                Console.WriteLine("=== Tipos de pensión ===");
                List<TipoPensionDTO> pensiones = servicio.ObtenerTiposPension();
                if (pensiones.Count == 0)
                    Console.WriteLine("No hay tipos de pensión definidos.");
                else
                    foreach (var p in pensiones)
                        Console.WriteLine($"ID: {p.TipoPensionId}, Tipo: {p.NombreTipo}");

                Console.WriteLine("\nPresiona cualquier tecla para salir...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al ejecutar la prueba: {ex.Message}");
                Console.WriteLine("Presiona cualquier tecla para salir...");
                Console.ReadKey();
            }
        }
    }
}
