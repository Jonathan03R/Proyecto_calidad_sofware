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
    public class PensionService
    {
        private readonly AccesoSQLServer accesoSQLServer;
        private readonly SistemasPensionesRepositorio pensionDAO;

        public PensionService()
        {
            accesoSQLServer = new AccesoSQLServer();
            pensionDAO = new SistemasPensionesRepositorio(accesoSQLServer);
        }

        public List<TipoPension> ObtenerSistemasPensiones()
        {
            List<TipoPension> listaPensiones;
            try
            {
                accesoSQLServer.AbrirConexion();
                listaPensiones = pensionDAO.ObtenerSistemasPensiones();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                accesoSQLServer.CerrarConexion();
            }
            return listaPensiones;
        }
    }
}
