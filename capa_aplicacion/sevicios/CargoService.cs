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
    public class CargoService
    {
        private readonly CargosRepositorio cargosDAO;
        private readonly AccesoSQLServer conexion;

        public CargoService()
        {
            conexion = new AccesoSQLServer();
            cargosDAO = new CargosRepositorio(conexion);

        }

        public List<Cargo> ObtenerCargos(int? cargoId = null, string cargoNombre = null)
        {
            List<Cargo> listaCargo;

            try
            {
                conexion.AbrirConexion();

                listaCargo = cargosDAO.ObtenerCargos();

                if (cargoId.HasValue)
                    listaCargo = listaCargo.Where(c => c.CargoId == cargoId.Value).ToList();

                if (!string.IsNullOrEmpty(cargoNombre))
                    listaCargo = listaCargo.Where(c => c.CargoNombre.Contains(cargoNombre)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en CargoService: " + ex.ToString());
                throw;  
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return listaCargo;
        }
    }
}

