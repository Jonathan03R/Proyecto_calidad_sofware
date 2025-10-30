using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{
    public class Parametros
    {
        private readonly AccesoSQLServer _accesoSQL;
        public Parametros() { _accesoSQL = new AccesoSQLServer(); }

        //public Parametro ObtenerParametroVigentePorCodigo(string codigo)
        //{
        //    try
        //    {
        //        _accesoSQL.AbrirConexion();
        //        var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("parametros.proc_obtener_parametro_por_codigo_vigente");
        //        cmd.Parameters.AddWithValue("@parametro_codigo", codigo);

        //        using (var reader = cmd.ExecuteReader())
        //            if (reader.Read())
        //            {
        //                return new Parametro
        //                {
        //                    ParametroId = reader.GetInt32(reader.GetOrdinal("parametro_id")),
        //                    ParametroCodigo = reader.GetString(reader.GetOrdinal("parametro_codigo")),
        //                    ParametroNombre = reader.GetString(reader.GetOrdinal("parametro_nombre")),
        //                    ParametroValor = reader.GetDecimal(reader.GetOrdinal("parametro_valor")),
        //                    ParametroFechaVigencia = reader.GetDateTime(reader.GetOrdinal("parametro_fecha_vigencia")),
        //                    ParametroEstado = reader.GetString(reader.GetOrdinal("parametro_estado"))
        //                };
        //            }
        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
        //    }
        //    finally { _accesoSQL.CerrarConexion(); }
        //}

        // C-05: lista de parámetros vigentes para nómina
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
                            //FechaVigencia = reader.GetDateTime(reader.GetOrdinal("parametro_fecha_vigencia"))
                        };
                        lista.Add(p);
                    }
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally { _accesoSQL.CerrarConexion(); }

            return lista;
        }
    }
}


