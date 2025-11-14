using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.sevicios.Tipos_salarios
{
    public class TipoSalarioServicio
    {
        private readonly AccesoSQLServer conexion;
        private readonly TiposSalariosRepositorio tiposSalariosRepositorio;

        public TipoSalarioServicio()
        {
            conexion = new AccesoSQLServer();
            tiposSalariosRepositorio = new TiposSalariosRepositorio(conexion);
        }

        public List<TipoSalario> ObtenerTiposSalarios()
        {
            List<TipoSalario> tiposSalariosData;
            try
            {
                conexion.AbrirConexion();
               
                tiposSalariosData = tiposSalariosRepositorio.ObtenerTiposSalarios();
              
                return tiposSalariosData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los tipos de salarios", ex);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
    }
}
