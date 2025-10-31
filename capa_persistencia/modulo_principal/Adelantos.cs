using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{

    public class AdelantoSueldodto
    {
        public int AdelantoId { get; set; }
        public int TrabajadorId { get; set; }
        public decimal AdelantoMonto { get; set; }
        public DateTime AdelantoFecha { get; set; }
        public string AdelantoMotivo { get; set; }    
        public string AdelantoObservaciones { get; set; } 
    }

    public class Adelantos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Adelantos()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<AdelantoSueldodto> ObtenerAdelantosPorTrabajador(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<AdelantoSueldodto>();

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
                        var a = new AdelantoSueldodto
                        {
                            AdelantoId = reader.GetInt32(reader.GetOrdinal("adelanto_id")),
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            AdelantoMonto = reader.GetDecimal(reader.GetOrdinal("adelanto_monto")),
                            AdelantoFecha = reader.GetDateTime(reader.GetOrdinal("adelanto_fecha")),
                            AdelantoMotivo = SafeGetString(reader, "adelanto_motivo"),
                            AdelantoObservaciones = SafeGetString(reader, "adelanto_observaciones")
                        };
                        lista.Add(a);
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

        private static string SafeGetString(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }
    }
}
