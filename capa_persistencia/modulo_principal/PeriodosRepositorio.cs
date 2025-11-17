using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using capa_dominio;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class PeriodosRepositorio
    {
        private readonly AccesoSQLServer _conexion;

        public PeriodosRepositorio(AccesoSQLServer conexion)
        {
            _conexion = conexion;
        }

        /// <summary>
        /// Lista períodos filtrando por ID o por nombre.
        /// </summary>
        public List<Periodo> ListarPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            var lista = new List<Periodo>();

            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_Listar_Periodos");
                cmd.Parameters.AddWithValue("@PeriodoID", (object)periodoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PeriodoNombre", (object)periodoNombre ?? DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Periodo
                        {
                            PeriodoId = Convert.ToInt32(dr["periodo_id"]),
                            PeriodoNombre = dr["periodo_nombre"].ToString(),
                            PeriodoFechaInicio = Convert.ToDateTime(dr["periodo_fecha_inicio"]),
                            PeriodoFechaFin = Convert.ToDateTime(dr["periodo_fecha_fin"]),
                            PeriodoEstado = dr["periodo_estado"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar períodos de nómina.", ex);
            }

            return lista;
        }

        public Periodo ObtenerPeriodoPorId(int periodoId)
        {
            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_Listar_Periodos");
                cmd.Parameters.AddWithValue("@PeriodoID", periodoId);
                cmd.Parameters.AddWithValue("@PeriodoNombre", DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Periodo
                        {
                            PeriodoId = Convert.ToInt32(dr["periodo_id"]),
                            PeriodoNombre = dr["periodo_nombre"].ToString(),
                            PeriodoFechaInicio = Convert.ToDateTime(dr["periodo_fecha_inicio"]),
                            PeriodoFechaFin = Convert.ToDateTime(dr["periodo_fecha_fin"]),
                            PeriodoEstado = dr["periodo_estado"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener periodo con ID {periodoId}.", ex);
            }

            return null;
        }
    }
}
