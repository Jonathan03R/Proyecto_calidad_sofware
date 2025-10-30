using System;
using System.Collections.Generic;
using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{

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
                            Trabajador = new Trabajador
                            {
                                TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id"))
                            },
                            FaltaFecha = reader.GetDateTime(reader.GetOrdinal("falta_fecha")),
// <<<<<<< CapaPresentacionH2
//                             FaltaTipo = reader.IsDBNull(reader.GetOrdinal("falta_tipo"))
//                                 ? string.Empty
//                                 : reader.GetString(reader.GetOrdinal("falta_tipo")),

//                             FaltaDias = reader.IsDBNull(reader.GetOrdinal("falta_dias"))
//                                 ? 0m
//                                 : reader.GetDecimal(reader.GetOrdinal("falta_dias")),

//                             FaltaValorDiaNormal = reader.IsDBNull(reader.GetOrdinal("falta_valor_dia_normal"))
//                                 ? 0m
//                                 : reader.GetDecimal(reader.GetOrdinal("falta_valor_dia_normal")),

//                             FaltaValorDescuento = reader.IsDBNull(reader.GetOrdinal("falta_valor_descuento"))
//                                 ? 0m
//                                 : reader.GetDecimal(reader.GetOrdinal("falta_valor_descuento")),

//                             FaltaObservaciones = reader.IsDBNull(reader.GetOrdinal("falta_observaciones"))
//                                 ? string.Empty
//                                 : reader.GetString(reader.GetOrdinal("falta_observaciones")),

//                             FaltaDocumentoSoporte = reader.IsDBNull(reader.GetOrdinal("falta_documento_soporte"))
//                                 ? string.Empty
//                                 : reader.GetString(reader.GetOrdinal("falta_documento_soporte"))
// =======
//                             FaltaTipo = SafeGetString(reader, "falta_tipo"),
//                             FaltaObservaciones = SafeGetString(reader, "falta_observaciones"),
//                             FaltaDocumentoSoporte = SafeGetString(reader, "falta_documento_soporte")
// >>>>>>> main
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
// <<<<<<< CapaPresentacionH2
// =======

//         // -----------------------
//         // Helpers NULL-safe
//         // -----------------------
//         private static string SafeGetString(SqlDataReader r, string col)
//         {
//             int i = r.GetOrdinal(col);
//             return r.IsDBNull(i) ? null : r.GetString(i);
//         }

//         // Devuelve decimal con valor por defecto si la columna es NULL
//         private static decimal SafeGetDecimalOrDefault(SqlDataReader r, string col, decimal defaultValue)
//         {
//             int i = r.GetOrdinal(col);
//             return r.IsDBNull(i) ? defaultValue : r.GetDecimal(i);
//         }
// >>>>>>> main
    }
}
