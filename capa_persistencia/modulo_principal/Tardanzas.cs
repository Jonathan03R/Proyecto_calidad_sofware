using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using capa_dominio;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class Tardanzas
    {
        private readonly string _cs;
        public Tardanzas(string connectionString) { _cs = connectionString; }

        public List<Tardanza> ObtenerPorTrabajador(int trabajadorId)
        {
            var lista = new List<Tardanza>();
            try
            {
                using (var cn = new SqlConnection(_cs))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand("nomina.proc_obtener_tardanzas_por_trabajador", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@trabajador_id", SqlDbType.Int).Value = trabajadorId;

                        using (var dr = cmd.ExecuteReader())
                        {
                            int iId = dr.GetOrdinal("tardanza_id");
                            int iTid = dr.GetOrdinal("trabajador_id");
                            int iFec = dr.GetOrdinal("tardanza_fecha");
                            int iMin = dr.GetOrdinal("tardanza_minutos");
                            int iHrs = dr.GetOrdinal("tardanza_horas");
                            int iObs = dr.GetOrdinal("tardanza_observaciones");

                            while (dr.Read())
                            {
                                lista.Add(new Tardanza
                                {
                                    TardanzaId = dr.GetInt32(iId),
                                    Trabajador = new Trabajador { TrabajadorId = dr.GetInt32(iTid) },
                                    TardanzaFecha = dr.GetDateTime(iFec),
                                    TardanzaMinutos = dr.GetInt32(iMin),
                                    TardanzaHoras = dr.GetDecimal(iHrs),
                                    TardanzaObservaciones = dr.IsDBNull(iObs) ? null : dr.GetString(iObs)
                                });
                            }
                        }
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA, ex.Message);
            }
        }
    }
}
