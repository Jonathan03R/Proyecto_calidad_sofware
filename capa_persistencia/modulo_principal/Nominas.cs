using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    // DTO (mover a Dominio si ya lo tienes allí)
    public class TrabajadorProcesable
    {
        public int TrabajadorId { get; set; }
        public string TrabajadorCodigo { get; set; }
        public string TrabajadorEstado { get; set; }
        public int ContratoId { get; set; }
        public int TipoPensionId { get; set; }
        public int TipoSalarioId { get; set; }
        public int TipoJornadaId { get; set; }
        public int EstadoContratoId { get; set; }
        public DateTime ContratoFechaInicio { get; set; }
        public DateTime? ContratoFechaFin { get; set; }
    }

    public class Nominas
    {
        private readonly AccesoSQLServer _accesoSQL;
        public Nominas() { _accesoSQL = new AccesoSQLServer(); }

        // I-01: Insertar cabecera -> devuelve nomina_id
        public int IniciarProcesoPorPeriodo(int periodoId, string observaciones = null)
        {
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_nomina_cabecera_por_periodo");
                cmd.Parameters.AddWithValue("@periodo_id", periodoId);
                cmd.Parameters.AddWithValue("@nomina_fecha", DBNull.Value); 
                cmd.Parameters.AddWithValue("@nomina_observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_estado", "Procesando");

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CREACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

        // M-01: Actualizar totales (void)
        public void ActualizarTotales(int nominaId, int totalEmpleados,
                                      decimal totalBruto, decimal totalDescuentos,
                                      decimal totalNeto, string estadoFinal)
        {
            try
            {
                _accesoSQL.AbrirConexion();

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
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_ACTUALIZACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

        // M-02: Actualizar estado (void)
        public void ActualizarEstado(int nominaId, string nuevoEstado)
        {
            try
            {
                _accesoSQL.AbrirConexion();

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
            finally { _accesoSQL.CerrarConexion(); }
        }

        // C-02: Trabajadores procesables por período
        public List<TrabajadorProcesable> ObtenerTrabajadoresPorPeriodo(int periodoId)
        {
            var lista = new List<TrabajadorProcesable>();
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_obtener_trabajadores_por_periodo_activo");
                cmd.Parameters.AddWithValue("@periodo_id", periodoId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var t = new TrabajadorProcesable
                        {
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            TrabajadorCodigo = reader.GetString(reader.GetOrdinal("trabajador_codigo")),
                            TrabajadorEstado = reader.GetString(reader.GetOrdinal("trabajador_estado")),
                            ContratoId = reader.GetInt32(reader.GetOrdinal("contrato_id")),
                            TipoPensionId = reader.GetInt32(reader.GetOrdinal("tipo_pension_id")),
                            TipoSalarioId = reader.GetInt32(reader.GetOrdinal("tipo_salario_id")),
                            TipoJornadaId = reader.GetInt32(reader.GetOrdinal("tipo_jornada_id")),
                            EstadoContratoId = reader.GetInt32(reader.GetOrdinal("estado_contrato_id")),
                            ContratoFechaInicio = reader.GetDateTime(reader.GetOrdinal("contrato_fecha_inicio")),
                            ContratoFechaFin = reader.IsDBNull(reader.GetOrdinal("contrato_fecha_fin"))
                                                  ? (DateTime?)null
                                                  : reader.GetDateTime(reader.GetOrdinal("contrato_fecha_fin"))
                        };
                        lista.Add(t);
                    }
                }
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally { _accesoSQL.CerrarConexion(); }

            return lista;
        }
    }
}
