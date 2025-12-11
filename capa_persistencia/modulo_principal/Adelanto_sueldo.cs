using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;


namespace capa_persistencia.modulo_principal
{
    public class Adelanto_sueldo
    {
        private readonly AccesoSQLServer _accesoSQL = new AccesoSQLServer();


        public List<AdelantoSueldo> ObtenerAdelantosPorTrabajador(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<AdelantoSueldo>();

            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_adelantos_por_trabajador");

                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);
                cmd.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fecha_fin", fechaFin);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var adelanto = new AdelantoSueldo
                        {
                            AdelantoId = reader.GetInt32(reader.GetOrdinal("adelanto_id")),

                            Trabajador = new Trabajador
                            {
                                TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id"))
                            },

                            AdelantoMonto = reader.GetDecimal(reader.GetOrdinal("adelanto_monto")),
                            AdelantoFecha = reader.GetDateTime(reader.GetOrdinal("adelanto_fecha")),
                            AdelantoMotivo = reader.IsDBNull(reader.GetOrdinal("adelanto_motivo")) ? null : reader.GetString(reader.GetOrdinal("adelanto_motivo")),
                            AdelantoObservaciones = reader.IsDBNull(reader.GetOrdinal("adelanto_observaciones")) ? null : reader.GetString(reader.GetOrdinal("adelanto_observaciones"))
                        };

                        lista.Add(adelanto);
                    }
                }
            }
            catch
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return lista;
        }

    }
}