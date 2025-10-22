using System;
using System.Collections.Generic;
using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    // DTO para nomina.tardanzas
    public class Tardanzas
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Tardanzas()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        // C-TA-01: historial por trabajador
        // SP: nomina.proc_obtener_tardanzas_por_trabajador(@trabajador_id)
        public List<Tardanza> ObtenerTardanzasPorTrabajador(int trabajadorId)
        {
            var lista = new List<Tardanza>();

            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_tardanzas_por_trabajador");
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var t = new Tardanza
                        {
                            TardanzaId = reader.GetInt32(reader.GetOrdinal("tardanza_id")),
                            Trabajador = new Trabajador
                            {
                                TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id"))
                            },
                            TardanzaFecha = reader.GetDateTime(reader.GetOrdinal("tardanza_fecha")),
                            TardanzaMinutos = reader.GetInt32(reader.GetOrdinal("tardanza_minutos")),
                            TardanzaHoras = reader.GetDecimal(reader.GetOrdinal("tardanza_horas")),
                            TardanzaValorHoraNormal = reader.GetDecimal(reader.GetOrdinal("tardanza_valor_hora_normal")),
                            TardanzaValorDescuento = reader.GetDecimal(reader.GetOrdinal("tardanza_valor_descuento")),
                            TardanzaObservaciones = SafeGetString(reader, "tardanza_observaciones")
                        };
                        lista.Add(t);
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

        // Helpers NULL-safe
        private static string SafeGetString(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }
    }
}
