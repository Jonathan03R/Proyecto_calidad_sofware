using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class NominasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;
        public NominasRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL;
        }

        public int IniciarProcesoPorPeriodo(int periodoId, string observaciones = null)
        {
            System.Diagnostics.Debug.WriteLine($"Iniciando proceso de nómina para el período ID: {periodoId}");
            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_nomina_cabecera_por_periodo");

                cmd.Parameters.AddWithValue("@periodo_id", periodoId);
                cmd.Parameters.AddWithValue("@nomina_fecha", DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_estado", "Procesando");

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CREACION);
            }
        }

        public void ActualizarTotales(
            int nominaId,
            int totalEmpleados,
            decimal totalBruto,
            decimal totalDescuentos,
            decimal totalNeto,
            string estadoFinal)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Actualizando totales de nómina ID: {nominaId}");
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_actualizar_nomina_totales_por_id");

                cmd.Parameters.AddWithValue("@nomina_id", nominaId);
                cmd.Parameters.AddWithValue("@total_empleados", totalEmpleados);
                cmd.Parameters.AddWithValue("@total_bruto", totalBruto);
                cmd.Parameters.AddWithValue("@total_descuentos", totalDescuentos);
                cmd.Parameters.AddWithValue("@total_neto", totalNeto);
                cmd.Parameters.AddWithValue("@estado_final", estadoFinal);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_ACTUALIZACION);
            }
        }

        public void ActualizarEstado(int nominaId, string nuevoEstado)
        {
            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_actualizar_nomina_estado_por_id");

                cmd.Parameters.AddWithValue("@nomina_id", nominaId);
                cmd.Parameters.AddWithValue("@nuevo_estado", nuevoEstado);

                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_ACTUALIZACION);
            }
        }

        public List<Nomina> ObtenerNominasEnPeriodo(int periodoId)
        {
            var nominas = new List<Nomina>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_obtener_nominas_por_periodo"
                );

                comando.Parameters.AddWithValue("@periodo_id", periodoId);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nomina = new Nomina
                        {
                            NominaId = reader.GetInt32(reader.GetOrdinal("nomina_id")),
                            Periodo = new Periodo
                            {
                                PeriodoId = reader.GetInt32(reader.GetOrdinal("periodo_id"))
                            },

                            NominaFecha = reader.IsDBNull(reader.GetOrdinal("nomina_fecha"))
                                ? DateTime.MinValue
                                : reader.GetDateTime(reader.GetOrdinal("nomina_fecha")),

                            NominaFechaProcesamiento = reader.IsDBNull(reader.GetOrdinal("nomina_fecha_procesamiento"))
                                ? DateTime.MinValue
                                : reader.GetDateTime(reader.GetOrdinal("nomina_fecha_procesamiento")),

                            NominaEstado = reader.GetString(reader.GetOrdinal("nomina_estado")),
                            NominaTotalEmpleados = reader.GetInt32(reader.GetOrdinal("nomina_total_empleados")),
                            NominaTotalBruto = reader.GetDecimal(reader.GetOrdinal("nomina_total_bruto")),
                            NominaTotalDescuentos = reader.GetDecimal(reader.GetOrdinal("nomina_total_descuentos")),
                            NominaTotalNeto = reader.GetDecimal(reader.GetOrdinal("nomina_total_neto")),
                            NominaObservaciones = reader.IsDBNull(reader.GetOrdinal("nomina_observaciones"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nomina_observaciones"))
                        };

                        nominas.Add(nomina);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error obteniendo nóminas del periodo {periodoId}: {ex.Message}"
                );
            }

            return nominas;
        }

        public PaginacionResultadoDTO<NominasProcesadasDTO> ListarHistorialPaginado(
            int page,
            int pageSize,
            int? periodoId = null,
            string estadoNomina = null,
            string buscar = null
        )
        {
            var resultado = new PaginacionResultadoDTO<NominasProcesadasDTO>();
            var lista = new List<NominasProcesadasDTO>();

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_listar_nominas_historial_paginado"
                );

                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@periodoId", (object)periodoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estadoNomina", (object)estadoNomina ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@buscar", (object)buscar ?? DBNull.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resultado.Total = reader.GetInt32(reader.GetOrdinal("total"));
                        resultado.TotalPages = reader.GetInt32(reader.GetOrdinal("totalPages"));
                        resultado.Page = reader.GetInt32(reader.GetOrdinal("page"));
                        resultado.PageSize = reader.GetInt32(reader.GetOrdinal("pageSize"));

                        var json = reader.GetString(reader.GetOrdinal("rowsJson"));
                        lista = System.Text.Json.JsonSerializer
                            .Deserialize<List<NominasProcesadasDTO>>(json);
                    }
                }

                resultado.Items = lista;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ListarHistorialPaginado] Error: {e.Message}"
                );
                throw;
            }

            return resultado;
        }
    }
}
