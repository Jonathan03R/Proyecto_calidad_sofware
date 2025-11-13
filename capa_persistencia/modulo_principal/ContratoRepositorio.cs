using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class ContratoRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public ContratoRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL;
        }

        public int CrearContratoEmpleado(ContratoDTO contrato)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_crear_contrato_empleado");

                comando.Parameters.AddWithValue("@trabajadorid", contrato.TrabajadorId);
                comando.Parameters.AddWithValue("@cargoid", contrato.CargoId);
                comando.Parameters.AddWithValue("@areaid", contrato.AreaId);
                comando.Parameters.AddWithValue("@tipopensionid", contrato.TipoPensionId);
                comando.Parameters.AddWithValue("@tiposalarioid", contrato.TipoSalarioId);
                comando.Parameters.AddWithValue("@tipojornadaid", contrato.TipoJornadaId);
                comando.Parameters.AddWithValue("@fechainicio", contrato.FechaInicio);
                comando.Parameters.AddWithValue("@fechafin", contrato.FechaFin ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@salario", contrato.Salario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@tarifahora", contrato.TarifaHora ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@modopago", contrato.ModoPago ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@documentourl", contrato.DocumentoUrl ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@descripcionfunciones", contrato.DescripcionFunciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@observaciones", contrato.Observaciones ?? (object)DBNull.Value);

                var result = comando.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }


        public (int contratoActualizado, int cambioRegistrado) FinalizarContrato(int contratoId, string observaciones = null)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_finalizar_contrato");

                comando.Parameters.AddWithValue("@contratoid", contratoId);
                comando.Parameters.AddWithValue("@observaciones", observaciones ?? (object)DBNull.Value);

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int contratoActualizado = reader.GetInt32(reader.GetOrdinal("contrato_actualizado"));
                        int cambioRegistrado = reader.GetInt32(reader.GetOrdinal("cambio_registrado"));
                        return (contratoActualizado, cambioRegistrado);
                    }
                }

                throw new Exception("Error al finalizar contrato: no se devolvieron resultados.");
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }

        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("actualizar_contrato");

                comando.Parameters.AddWithValue("@contrato_id", contratoId);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@motivo", motivo);
                comando.Parameters.AddWithValue("@observaciones", contrato.Observaciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@cargo_id", contrato.CargoId ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@tipo_salario_id", contrato.TipoSalarioId ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_salario", contrato.Salario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_modo_pago", contrato.ModoPago ?? (object)DBNull.Value);

                comando.ExecuteNonQuery();
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }


        public List<Contrato> ObtenerContratosPorTrabajador(int trabajadorId)
        {
            var contratos = new List<Contrato>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_contratos_por_trabajador");
                comando.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var contrato = new Contrato
                        {
                            ContratoId = reader.GetInt32(reader.GetOrdinal("contrato_id")),
                            ContratoFechaInicio = reader.GetDateTime(reader.GetOrdinal("contrato_fecha_inicio")),
                            ContratoFechaFin = reader.IsDBNull(reader.GetOrdinal("contrato_fecha_fin"))
                                ? (DateTime?)null
                                : reader.GetDateTime(reader.GetOrdinal("contrato_fecha_fin")),
                            ContratoSalario = reader.IsDBNull(reader.GetOrdinal("contrato_salario"))
                                ? 0
                                : reader.GetDecimal(reader.GetOrdinal("contrato_salario")),
                            ContratoTarifaHora = reader.IsDBNull(reader.GetOrdinal("contrato_tarifa_hora"))
                                ? 0
                                : reader.GetDecimal(reader.GetOrdinal("contrato_tarifa_hora")),
                            ContratoHorasSemanales = reader.IsDBNull(reader.GetOrdinal("contrato_horas_semanales"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("contrato_horas_semanales")),
                            ContratoModoPago = reader["contrato_modo_pago"]?.ToString(),
                            ContratoDocumentoUrl = reader["contrato_documento_url"]?.ToString(),
                            ContratoDescripcionFunciones = reader["contrato_descripcion_funciones"]?.ToString(),
                            ContratoObservaciones = reader["contrato_observaciones"]?.ToString(),
                            EstadoiId = reader.GetInt32(reader.GetOrdinal("estado_contrato_id")),
                        };

                        // ✅ Mapea solo el ID del tipo de pensión
                        if (!reader.IsDBNull(reader.GetOrdinal("tipo_pension_id")))
                        {
                            contrato.TipoPension = new TipoPension
                            {
                                TipoPensionId = reader.GetInt32(reader.GetOrdinal("tipo_pension_id")),
                                Nombre = reader["tipo_pension_nombre"]?.ToString(),
                                Entidad = reader["tipo_pension_entidad"]?.ToString()
                            };
                        }

                        contratos.Add(contrato);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener contratos: {ex.Message}");
                throw;
            }

            return contratos;
        }
    }
}
