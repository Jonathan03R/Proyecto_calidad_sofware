using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class TiposJornadasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TiposJornadasRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL;
        }

        public List<TipoJornada> ObtenerTiposJornadas()
        {
            var tipos = new List<TipoJornada>();

            try
            {
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
                throw new TrabajadorException(TrabajadorException.ERROR_DE_CONSULTA);
            }

            return tipos;
        }
    }
}