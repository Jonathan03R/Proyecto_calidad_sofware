using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    // DTO para el resultado de personal.proc_Obtener_Horas_Trabajadas
    public class HorasTrabajadasDetalle
    {
        public string NombreCompleto { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }       
        public TimeSpan? HoraSalida { get; set; }     
        public decimal HorasNormales { get; set; }
        public decimal HorasExtra { get; set; }
        public decimal HorasDescanso { get; set; }
        public decimal TotalDiario { get; set; }
        public string Observaciones { get; set; }
    }

    public class HorasTrabajadas
    {
        private readonly AccesoSQLServer _accesoSQL;

        public HorasTrabajadas()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<HorasTrabajadasDetalle> ObtenerHorasTrabajadas(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<HorasTrabajadasDetalle>();

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
                        var item = new HorasTrabajadasDetalle
                        {
                            NombreCompleto = SafeGetString(reader, "NombreCompleto"),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                            HoraEntrada = SafeGetTime(reader, "HoraEntrada"),
                            HoraSalida = SafeGetTime(reader, "HoraSalida"),
                            HorasNormales = reader.GetDecimal(reader.GetOrdinal("HorasNormales")),
                            HorasExtra = reader.GetDecimal(reader.GetOrdinal("HorasExtra")),
                            HorasDescanso = reader.GetDecimal(reader.GetOrdinal("HorasDescanso")),
                            TotalDiario = reader.GetDecimal(reader.GetOrdinal("TotalDiario")),
                            Observaciones = SafeGetString(reader, "Observaciones")
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
        // Helpers NULL-safe
        // -----------------------
        private static string SafeGetString(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        private static TimeSpan? SafeGetTime(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (TimeSpan?)null : r.GetTimeSpan(i);
        }
    }
}
