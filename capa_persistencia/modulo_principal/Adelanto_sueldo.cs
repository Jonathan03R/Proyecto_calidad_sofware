using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using capa_dominio;                       // AdelantoSueldo, Trabajador
using capa_persistencia.modulo_base;     // AccesoSQLServer
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class Adelantos_Repositorio
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
                            AdelantoMotivo = SafeGetString(reader, "adelanto_motivo"),
                            AdelantoObservaciones = SafeGetString(reader, "adelanto_observaciones")
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

        // -----------------------------
        // Helpers seguros para NULL
        // -----------------------------
        private static string SafeGetString(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        /// <summary>
        /// adelantos de sueldo de un trabajador si es null , no tiene adelanto de sueldo
        /// </summary>
        /// <param name="trabajadorId"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        /// <exception cref="ExcepcionTrabajador"></exception>
        public List<AdelantoSueldo> ObtenerAdelantosPorTrabajador(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var adelantos = new List<AdelantoSueldo>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_adelantos_por_trabajador");
                comando.Parameters.AddWithValue("@trabajador_id", trabajadorId);
                comando.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                comando.Parameters.AddWithValue("@fecha_fin", fechaFin);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var adelanto = new AdelantoSueldo
                        {
                            AdelantoId = reader.GetInt32(reader.GetOrdinal("adelanto_id")),
                            AdelantoMonto = reader.GetDecimal(reader.GetOrdinal("adelanto_monto")),
                            AdelantoFecha = reader.GetDateTime(reader.GetOrdinal("adelanto_fecha")),
                            AdelantoMotivo = reader.GetString(reader.GetOrdinal("adelanto_motivo")),
                            AdelantoObservaciones = reader.IsDBNull(reader.GetOrdinal("adelanto_observaciones")) ? null : reader.GetString(reader.GetOrdinal("adelanto_observaciones"))
                        };
                        adelantos.Add(adelanto);
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

            return adelantos;
        }
       
    }
}