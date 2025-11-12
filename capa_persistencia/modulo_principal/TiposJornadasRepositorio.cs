using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class TiposJornadasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TiposJornadasRepositorio()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<TipoJornada> ObtenerTiposJornadas()
        {
            var tipos = new List<TipoJornada>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_tipo_jornal");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tipo = new TipoJornada
                        {
                            TipoJornadaId = reader.GetInt32(reader.GetOrdinal("tipo_jornada_id")),
                            TipoJornadaNombre = reader.GetString(reader.GetOrdinal("tipo_jornada_nombre")),
                            TipoJornadaDescripcion = reader.GetString(reader.GetOrdinal("tipo_jornada_descripcion"))
                        };
                        tipos.Add(tipo);
                    }
                }
            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return tipos;
        }
    }
}