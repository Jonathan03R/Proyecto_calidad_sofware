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
            try
            {
                conexion.AbrirConexion();
                return tiposSalariosRepositorio.ObtenerTiposSalarios();
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
    }
}