using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class Parametros
    {
        private readonly AccesoSQLServer _accesoSQL;
        public class ParametroVigente
        {
            public int ParametroId { get; set; }          
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public decimal Valor { get; set; }
            public DateTime FechaVigencia { get; set; }
            public string Estado { get; set; }
        }
        public Parametros() { _accesoSQL = new AccesoSQLServer(); }

        // C-03: TOP 1 parámetro vigente por código
        public ParametroVigente ObtenerParametroVigentePorCodigo(string codigo)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("parametros.proc_obtener_parametro_por_codigo_vigente");
                cmd.Parameters.AddWithValue("@parametro_codigo", codigo);

                using (var reader = cmd.ExecuteReader()) 
                if (reader.Read())
                {
                    return new ParametroVigente
                    {
                        ParametroId = reader.GetInt32(reader.GetOrdinal("parametro_id")),
                        Codigo = reader.GetString(reader.GetOrdinal("parametro_codigo")),
                        Nombre = reader.GetString(reader.GetOrdinal("parametro_nombre")),
                        Valor = reader.GetDecimal(reader.GetOrdinal("parametro_valor")),
                        FechaVigencia = reader.GetDateTime(reader.GetOrdinal("parametro_fecha_vigencia")),
                        Estado = reader.GetString(reader.GetOrdinal("parametro_estado"))
                    };
                }
                return null;
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

        // C-05: lista de parámetros vigentes para nómina
        public List<ParametroVigente> ListarParametrosVigentesParaNomina()
        {
            var lista = new List<ParametroVigente>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("parametros.proc_listar_parametros_vigentes_para_nomina");

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        var p = new ParametroVigente
                        {
                            Codigo = reader.GetString(reader.GetOrdinal("parametro_codigo")),
                            Nombre = reader.GetString(reader.GetOrdinal("parametro_nombre")),
                            Valor = reader.GetDecimal(reader.GetOrdinal("parametro_valor")),
                            FechaVigencia = reader.GetDateTime(reader.GetOrdinal("parametro_fecha_vigencia"))
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

        // rubi tienes chamba

        public List<ParametroVigente> AsignacionFamiliar()
        {

        }
    }
}


