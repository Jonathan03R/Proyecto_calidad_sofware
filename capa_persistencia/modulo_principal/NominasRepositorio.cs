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
                throw new NominaException(NominaException.ERROR_DE_CREACION);
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
                throw new NominaException(NominaException.ERROR_DE_ACTUALIZACION);
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
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"Error actualizando estado de nómina ID {nominaId}: {ex.Message}");
                throw new NominaException(NominaException.ERROR_DE_ACTUALIZACION);
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
        public List<ResumenNominaDto> ListarResumenNominas()
        {
            var lista = new List<ResumenNominaDto>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_listar_resumen_nominas");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var resumen = new ResumenNominaDto
                        {
                            PeriodoNombre = reader.GetString(reader.GetOrdinal("periodo_nombre")),
                            NominaFechaProcesamiento = reader.GetDateTime(reader.GetOrdinal("nomina_fecha_procesamiento")),
                            NominaTotalEmpleados = reader.GetInt32(reader.GetOrdinal("nomina_total_empleados")),
                            NominaEstado = reader.GetString(reader.GetOrdinal("nomina_estado")),
                            NominaTotalNeto = reader.GetDecimal(reader.GetOrdinal("nomina_total_neto"))
                        };

                        lista.Add(resumen);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error al consultar el resumen de nóminas: {ex}");

                throw new NominaException(
                    NominaException.ERROR_DE_CONSULTA,
                    "Error al consultar el resumen de nóminas."
                );
            }

            return lista;
        }
        public ResumenKpisNominaDto ObtenerResumenKpisNomina()
        {

            try
            {
                var resumen = new ResumenKpisNominaDto();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_resumen_kpis_nomina");

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resumen.TotalPeriodosAbiertos = reader.IsDBNull(reader.GetOrdinal("total_periodos_abiertos"))
                            ? 0 : reader.GetInt32(reader.GetOrdinal("total_periodos_abiertos"));

                        resumen.TotalPeriodosProcesados = reader.IsDBNull(reader.GetOrdinal("total_periodos_procesados"))
                            ? 0 : reader.GetInt32(reader.GetOrdinal("total_periodos_procesados"));

                        resumen.TotalTrabajadoresInactivos = reader.IsDBNull(reader.GetOrdinal("total_trabajadores_inactivos"))
                            ? 0 : reader.GetInt32(reader.GetOrdinal("total_trabajadores_inactivos"));

                        resumen.TotalNetoGeneral = reader.IsDBNull(reader.GetOrdinal("total_neto_general"))
                            ? 0 : reader.GetDecimal(reader.GetOrdinal("total_neto_general"));
                    }
                }

                return resumen;
            }
            catch (Exception )
            {
                throw new NominaException(
                    NominaException.ERROR_DE_CONSULTA
                );
            }

        }


    }
}
