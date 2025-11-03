using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{
    public class Contratos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Contratos()
        {
            _accesoSQL = new AccesoSQLServer();
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
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CREACION);
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
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_ACTUALIZACION);
            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_ACTUALIZACION);
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
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_ACTUALIZACION);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }

        public List<ContratoDTO> ObtenerContratosPorTrabajador(int trabajadorId)
        {
            List<ContratoDTO> contratos = new List<ContratoDTO>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_contratos_por_trabajador");
                comando.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ContratoDTO contrato = new ContratoDTO
                        {
                            TrabajadorId = reader.IsDBNull(reader.GetOrdinal("TrabajadorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TrabajadorId")),
                            CargoId = reader.IsDBNull(reader.GetOrdinal("CargoId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CargoId")),
                            AreaId = reader.IsDBNull(reader.GetOrdinal("AreaId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AreaId")),
                            TipoPensionId = reader.IsDBNull(reader.GetOrdinal("TipoPensionId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TipoPensionId")),
                            TipoSalarioId = reader.IsDBNull(reader.GetOrdinal("TipoSalarioId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TipoSalarioId")),
                            TipoJornadaId = reader.IsDBNull(reader.GetOrdinal("TipoJornadaId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TipoJornadaId")),
                            FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                            FechaFin = reader.IsDBNull(reader.GetOrdinal("FechaFin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                            Salario = reader.IsDBNull(reader.GetOrdinal("Salario")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Salario")),
                            TarifaHora = reader.IsDBNull(reader.GetOrdinal("TarifaHora")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TarifaHora")),
                            ModoPago = reader.IsDBNull(reader.GetOrdinal("ModoPago")) ? null : reader["ModoPago"].ToString(),
                            DocumentoUrl = reader.IsDBNull(reader.GetOrdinal("DocumentoUrl")) ? null : reader["DocumentoUrl"].ToString(),
                            DescripcionFunciones = reader.IsDBNull(reader.GetOrdinal("DescripcionFunciones")) ? null : reader["DescripcionFunciones"].ToString(),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader["Observaciones"].ToString()
                        };

                        contratos.Add(contrato);
                    }
                }
            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return contratos;
        }

    }
}