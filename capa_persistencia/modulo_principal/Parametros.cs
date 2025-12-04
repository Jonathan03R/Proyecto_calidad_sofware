using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{
    public class ParametrosRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;
        public ParametrosRepositorio() { _accesoSQL = new AccesoSQLServer(); }

       
        public List<Parametro> ListarParametrosVigentesParaNomina()
        {
            var lista = new List<Parametro>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("parametros.proc_listar_parametros_vigentes_para_nomina");

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        var p = new Parametro
                        {
                            ParametroId = reader.GetInt32(reader.GetOrdinal("parametro_id")),
                            ParametroCodigo = reader.GetString(reader.GetOrdinal("parametro_codigo")),
                            ParametroNombre = reader.GetString(reader.GetOrdinal("parametro_nombre")),
                            ParametroValor = reader.GetDecimal(reader.GetOrdinal("parametro_valor")),
                            ParametroFechaVigencia = reader.GetDateTime(reader.GetOrdinal("parametro_fecha_vigencia")),
                        };
                        lista.Add(p);
                    }
            }
            catch (Exception)
            {
                throw new NominaException(NominaException.ERROR_DE_CONSULTA);
            }
            finally { _accesoSQL.CerrarConexion(); }

            return lista;
        }
  }
}


