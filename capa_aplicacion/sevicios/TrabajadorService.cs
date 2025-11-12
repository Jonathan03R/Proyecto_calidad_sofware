using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.sevicios
{
    public class TrabajadorService
    {
        private readonly TrabajadoresRepositorio trabajadorDAO;
        private readonly AccesoSQLServer conexion;

        public TrabajadorService()
        {
            conexion = new AccesoSQLServer();
            trabajadorDAO = new TrabajadoresRepositorio(conexion);
        }

        public List<Trabajador> ObtenerEmpleados(int? pagina = null, int? cantidad = null)
        {
            List<Trabajador> listaTrabajadores;
            try
            {
                listaTrabajadores = trabajadorDAO.ObtenerEmpleados(pagina, cantidad);
            }
            catch (Exception ex)
            {
                // ✅ NO vuelvas a lanzar la excepción sin mostrar el error real
                System.Diagnostics.Debug.WriteLine("Error en TrabajadorService: " + ex.ToString());
                throw;  // ← Cambia "throw ex;" por solo "throw" para mantener el stack trace
            }
            return listaTrabajadores;
        }
    }
}
