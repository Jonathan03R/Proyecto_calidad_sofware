using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;

namespace capa_aplicacion.sevicios
{
    public class TipoJornadaService
    {
        private readonly AccesoSQLServer _conexion;
        private readonly TiposJornadasRepositorio _jornadaRepo;

        public TipoJornadaService()
        {
            _conexion = new AccesoSQLServer();
            _jornadaRepo = new TiposJornadasRepositorio(_conexion);
        }

        public List<TipoJornada> ObtenerTiposJornadas()
        {
            try
            {
                _conexion.AbrirConexion();
                return _jornadaRepo.ObtenerTiposJornadas();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener tipos de jornada: {ex.Message}");
                throw; 
            }

            finally
            {
                _conexion.CerrarConexion();
            }
        }
    }
}
