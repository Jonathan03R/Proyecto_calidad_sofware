using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

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
                            while (dr.Read())
                            {
                                var t = new Tardanza
                                {
                                    TardanzaId = dr.GetInt32(dr.GetOrdinal("tardanza_id")),
                                    Trabajador = new Trabajador
                                    {
                                        TrabajadorId = dr.GetInt32(dr.GetOrdinal("trabajador_id"))
                                    },
                                    TardanzaFecha = dr.GetDateTime(dr.GetOrdinal("tardanza_fecha")),

                                    TardanzaMinutos = dr.IsDBNull(dr.GetOrdinal("tardanza_minutos"))
                                                        ? 0
                                                        : dr.GetInt32(dr.GetOrdinal("tardanza_minutos")),

                                    TardanzaHoras = dr.IsDBNull(dr.GetOrdinal("tardanza_horas"))
                                                        ? 0m
                                                        : dr.GetDecimal(dr.GetOrdinal("tardanza_horas")),

                                    TardanzaValorHoraNormal = dr.IsDBNull(dr.GetOrdinal("tardanza_valor_hora_normal"))
                                                        ? 0m
                                                        : dr.GetDecimal(dr.GetOrdinal("tardanza_valor_hora_normal")),

                                    TardanzaValorDescuento = dr.IsDBNull(dr.GetOrdinal("tardanza_valor_descuento"))
                                                        ? 0m
                                                        : dr.GetDecimal(dr.GetOrdinal("tardanza_valor_descuento")),

                                    TardanzaObservaciones = dr.IsDBNull(dr.GetOrdinal("tardanza_observaciones"))
                                                        ? string.Empty
                                                        : dr.GetString(dr.GetOrdinal("tardanza_observaciones"))
                                };

                                if (t.TardanzaValorHoraNormal == 0m || t.TardanzaValorDescuento == 0m)
                                    t.CalcularDescuentoTardanza();

                                lista.Add(t);
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
