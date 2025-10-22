using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    // DTO tipado para nomina.faltas
    public class Falta
    {
        public int FaltaId { get; set; }
        public int TrabajadorId { get; set; }
        public DateTime FaltaFecha { get; set; }
        public string FaltaTipo { get; set; }
        public decimal? FaltaDias { get; set; }
        public decimal FaltaValorDiaNormal { get; set; }
        public decimal FaltaValorDescuento { get; set; }
        public string FaltaObservaciones { get; set; }
        public string FaltaDocumentoSoporte { get; set; }
    }

    public class Faltas
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Faltas()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        // -----------------------------------------------------------
        // C-FA-01: Obtener faltas de un trabajador (solo lectura)
        // Procedimiento: nomina.proc_obtener_faltas_por_trabajador
        // -----------------------------------------------------------
        public List<Falta> ObtenerFaltasPorTrabajador(int trabajadorId)
        {
            var lista = new List<Falta>();

            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_faltas_por_trabajador");
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var f = new Falta
                        {
                            FaltaId = reader.GetInt32(reader.GetOrdinal("falta_id")),
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            FaltaFecha = reader.GetDateTime(reader.GetOrdinal("falta_fecha")),
                            FaltaTipo = SafeGetString(reader, "falta_tipo"),
                            FaltaDias = SafeGetDecimal(reader, "falta_dias"),
                            FaltaValorDiaNormal = reader.GetDecimal(reader.GetOrdinal("falta_valor_dia_normal")),
                            FaltaValorDescuento = reader.GetDecimal(reader.GetOrdinal("falta_valor_descuento")),
                            FaltaObservaciones = SafeGetString(reader, "falta_observaciones"),
                            FaltaDocumentoSoporte = SafeGetString(reader, "falta_documento_soporte")
                        };
                        lista.Add(f);
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
        // Helpers para NULL-safe
        // -----------------------
        private static string SafeGetString(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        private static decimal? SafeGetDecimal(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (decimal?)null : r.GetDecimal(i);
        }
    
}
}
