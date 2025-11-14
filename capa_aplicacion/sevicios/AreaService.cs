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
    public class AreaService
    {
        private readonly AreasRepositorio areaRepositorio;
        private readonly AccesoSQLServer conexion;
        

        public AreaService()
        {
            conexion = new AccesoSQLServer();
            areaRepositorio = new AreasRepositorio(conexion);
        }

        public List<Area> ObtenerAreas()
        {
            List<Area> listaAreas;
            try
            {
                conexion.AbrirConexion();
                listaAreas = areaRepositorio.ObtenerAreas();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener areas{ex.Message}");
                throw ex;
            }
            finally
            {
                conexion.CerrarConexion();
            }
            return listaAreas;
        }
    }
}
