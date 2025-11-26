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
        public List<Periodo> ListarTodosPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            var lista = new List<Periodo>();

            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_listar_periodos");
                cmd.Parameters.AddWithValue("@periodo_id", (object)periodoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@periodo_nombre", (object)periodoNombre ?? DBNull.Value);

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
                            EstadoId = Convert.ToInt32(dr["periodo_estado_id"]),
                            EstadoNombre = dr["estado_nombre"].ToString(),
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
                cmd.Parameters.AddWithValue("@periodo_id", periodoId);
                cmd.Parameters.AddWithValue("@periodo_nombre", DBNull.Value);

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
                            EstadoId = Convert.ToInt32(dr["periodo_estado_id"]),
                            EstadoNombre = dr["estado_nombre"].ToString(),
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


        /// obtener periodos con estado id = 1  (pendientes)
        /// 
        public List<Periodo> ListarPeriodosPendientes()
        {
            var lista = new List<Periodo>();
            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_listar_periodos");
                cmd.Parameters.AddWithValue("@estado_id", 1); // Estado pendiente
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
                            EstadoId = Convert.ToInt32(dr["periodo_estado_id"]),
                            EstadoNombre = dr["estado_nombre"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar períodos pendientes de nómina.", ex);
            }
            return lista;
        }

        /// obtener periodos con estado id = 2  (procesados)
        public List<Periodo> ListarPeriodosProcesados()
        {
            var lista = new List<Periodo>();
            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_listar_periodos");
                cmd.Parameters.AddWithValue("@estado_id", 3); // Estado procesado
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
                            EstadoId = Convert.ToInt32(dr["periodo_estado_id"]),
                            EstadoNombre = dr["estado_nombre"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar períodos procesados de nómina.", ex);
            }
            return lista;
        }


        /// obtener periodos con estado id = 3  (procesados)
        public List<Periodo> ListarPeriodosAbiertos()
        {
            var lista = new List<Periodo>();
            try
            {
                SqlCommand cmd = _conexion.ObtenerComandoDeProcedimiento("proc_listar_periodos");
                cmd.Parameters.AddWithValue("@estado_id", 2); // Estado anulado
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
                            EstadoId = Convert.ToInt32(dr["periodo_estado_id"]),
                            EstadoNombre = dr["estado_nombre"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar períodos anulados de nómina.", ex);
            }
            return lista;
        }


        /// actualizar estado del periodo a 3 (procesado)
        public void ProcesarPeriodo(int periodoId)
        {
            try
            {
                string sql = @"
                    update nomina.periodos
                    set periodo_estado_id = 3
                    where periodo_id = @periodo_id;
                ";

                SqlCommand cmd = _conexion.ObtenerComandoSQL(sql);
                cmd.Parameters.AddWithValue("@periodo_id", periodoId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("error al procesar el periodo.", ex);
            }
        }
    }
}
