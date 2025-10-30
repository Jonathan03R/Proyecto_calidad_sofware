using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;

namespace PruebaConexion
{
    class Program
    {
        static void Main(string[] args)
        {
            var acceso = new AccesoSQLServer();

            try
            {
                acceso.AbrirConexion();
                Console.WriteLine("¡Conexión correcta!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de conexión: " + ex.Message);
            }
            finally
            {
                acceso.CerrarConexion();
            }

            // ------------------------------------------------
            // 2. Test de ConsultarNominaPorPeriodo
            // ------------------------------------------------
            //const int PeriodoID_a_Probar = 1;
            //const int CargoID_a_Probar = 1;

            //ReporteNomina reporteNomina = new ReporteNomina(); // Asumiendo que esta clase contiene el método

            //try
            //{
            //    // PRUEBA 1
            //    Console.WriteLine($"**[PRUEBA] Consultar nómina para el Período ID {PeriodoID_a_Probar} (sin filtro de CargoID)**");
            //    List<DetalleNomina> detalles = reporteNomina.ConsultarNominaPorPeriodo(PeriodoID_a_Probar, CargoID_a_Probar);

            //    Console.WriteLine($"✅ Encontrados {detalles.Count} detalles de nómina para el Período {PeriodoID_a_Probar}:");
            //    if (detalles.Count > 0)
            //    {
            //        foreach (var d in detalles)
            //        {
            //            Console.WriteLine($"- Código: {d.Trabajador.Codigo} | Nombres Completos: {d.Trabajador.Nombres} " +
            //                $"| Tipo de Identificacion: {d.Trabajador.TipoIdentificacion}" + $" | Identificacion: {d.Trabajador.Identificacion} " +
            //                $"| NetoPagar: {d.NetoPagar} | Sueldo Basico: {d.SueldoBasico} | Fecha Contrato: {d.Contrato.ContratoFechaInicio}" +
            //                $"| Fecha Fin Contrato: {d.Contrato.ContratoFechaFin} | Asignacion Familiar: {d.AsignacionFamiliar} " +
            //                $"| Monto Horas Extras: {d.HorasExtras} | Montos Bonos: {d.Bonos} | Otros Ingresos: {d.OtrosIngresos}" +
            //                $"| Aporte ESSalud: {d.AporteEssalud} | Aporte ONP: {d.AporteONP} | Aporte AFP: {d.DescuentoAFP}" +
            //                $"| RetencionImpuestoRenta: {d.ImpuestoRentaMensual} | DescuentoTardanzas: {d.DescuentoTardanzas} | DescuentoFaltas: {d.DescuentoFaltas}" +
            //                $"| DescuentoAdelantos: {d.DescuentoAdelantos} | TotalHaberes: {d.TotalIngresos} | TotalDescuentos: {d.TotalDescuentos}" );

            //        }
            //    }

            //    else
            //    {
            //        Console.WriteLine("⚠️ No se encontraron detalles de nómina con ese filtro de CargoID.");
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"\n🛑 ERROR al consultar nómina: {ex.Message}");
            //    if (ex.InnerException != null)
            //    {
            //        Console.WriteLine($"Detalle del Error: {ex.InnerException.Message}");
            //    }
            //}
            //finally
            //{
            //    // La conexión se cierra en el destructor de ReporteNomina si es necesario.
            //}

            //// 1. Instanciar la clase ReporteNomina (Esto abre la conexión en el constructor)
            //ReporteNomina reporteNomina = new ReporteNomina();

            //try
            //{
            //    // 2. Ejecutar el método
            //    // Llama a ListarPeriodos sin parámetros para listar todos los activos.
            //    List<Periodo> periodos = reporteNomina.ListarPeriodos();

            //    // 3. Imprimir y verificar los resultados
            //    Console.WriteLine($"✅ Encontrados {periodos.Count} períodos:");
            //    Console.WriteLine("----------------------------------------------------------------");

            //    if (periodos.Count == 0)
            //    {
            //        Console.WriteLine("⚠️ No se encontraron períodos activos. (Revisa el filtro 'periodo_estado = 'A' en el SP)");
            //    }
            //    else
            //    {
            //        foreach (var p in periodos)
            //        {
            //            Console.WriteLine($"ID: {p.PeriodoId}, Nombre: {p.PeriodoNombre}, Inicio: {p.PeriodoFechaInicio:d}, Fin: {p.PeriodoFechaFin:d}, Estado: {p.PeriodoEstado}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"\n🛑 ERROR al listar períodos: {ex.Message}");
            //    // Si hay un error de conexión, el error real estará en InnerException.
            //    if (ex.InnerException != null)
            //    {
            //        Console.WriteLine($"Detalle del Error: {ex.InnerException.Message}");
            //    }
            //}
            //finally
            //{
            //    // Si la conexión no se reabre dentro de ListarPeriodos, 
            //    // el método ya cerró la conexión en su propio bloque finally.
            //}

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
