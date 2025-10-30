using System;
using System.Collections.Generic;
using capa_dominio;                      
using capa_persistencia.modulo_base; 
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class HorasTrabajadas_Repositorio
    {
        private readonly AccesoSQLServer _accesoSQL = new AccesoSQLServer();

        public List<HoraTrabajada> ObtenerHorasTrabajadas(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<HoraTrabajada>();

            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("personal.proc_Obtener_Horas_Trabajadas");
                cmd.Parameters.AddWithValue("@TrabajadorID", trabajadorId);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount == 2
                        && string.Equals(reader.GetName(0), "Resultado", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(reader.GetName(1), "Mensaje", StringComparison.OrdinalIgnoreCase))
                    {
                        if (reader.Read())
                        {
                            var msg = reader.IsDBNull(1) ? "Error al obtener horas trabajadas." : reader.GetString(1);
                            throw new Exception(msg);
                        }
                    }

                    while (reader.Read())
                    {
                        var item = new HoraTrabajada
                        {
                            HoraTrabajadaId = SafeGetInt32(reader, "HoraTrabajadaId") ?? 0,
                            Trabajador = SafeGetInt32(reader, "TrabajadorId") is int cid
                                ? new Trabajador { TrabajadorId = cid }
                                : null,
                            HoraTrabajadaFecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                            HoraTrabajadaHoraEntrada = SafeGetTime(reader, "HoraEntrada"),
                            HoraTrabajadaHoraSalida = SafeGetTime(reader, "HoraSalida"),

                            HorasTrabajadas = reader.GetDecimal(reader.GetOrdinal("HorasNormales")),
                            HoraTrabajadaDescanso = reader.GetDecimal(reader.GetOrdinal("HorasDescanso")),
                            HoraTrabajadaObservaciones = SafeGetString(reader, "Observaciones"),
                            RegistroFechaCreacion = SafeGetDateTime(reader, "RegistroFechaCreacion") ?? default
                        };

                        lista.Add(item);
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

        // -----------------------
        // Helpers NULL/Columna-safe
        // -----------------------
        private static bool HasColumn(SqlDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static string SafeGetString(SqlDataReader r, string col)
        {
            if (!HasColumn(r, col)) return null;
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        private static TimeSpan? SafeGetTime(SqlDataReader r, string col)
        {
            if (!HasColumn(r, col)) return null;
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (TimeSpan?)null : r.GetTimeSpan(i);
        }

        private static DateTime? SafeGetDateTime(SqlDataReader r, string col)
        {
            if (!HasColumn(r, col)) return null;
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (DateTime?)null : r.GetDateTime(i);
        }

        private static int? SafeGetInt32(SqlDataReader r, string col)
        {
            if (!HasColumn(r, col)) return null;
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (int?)null : r.GetInt32(i);
        }
    }
}
