using System;
using capa_persistencia.modulo_base;

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

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
