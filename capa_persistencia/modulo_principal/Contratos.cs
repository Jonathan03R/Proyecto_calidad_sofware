using System;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class Contratos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Contratos()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public int CrearContratoEmpleado(int trabajadorId, int cargoId, int areaId, int tipoPensionId, int tipoSalarioId, int tipoJornadaId,
            DateTime fechaInicio, DateTime? fechaFin = null, decimal? salario = null, decimal? tarifaHora = null,
            string modoPago = "transferencia", string documentoUrl = null, string descripcionFunciones = null, string observaciones = null)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_crear_contrato_empleado");

                comando.Parameters.AddWithValue("@trabajadorid", trabajadorId);
                comando.Parameters.AddWithValue("@cargoid", cargoId);
                comando.Parameters.AddWithValue("@areaid", areaId);
                comando.Parameters.AddWithValue("@tipopensionid", tipoPensionId);
                comando.Parameters.AddWithValue("@tiposalarioid", tipoSalarioId);
                comando.Parameters.AddWithValue("@tipojornadaid", tipoJornadaId);
                comando.Parameters.AddWithValue("@fechainicio", fechaInicio);
                comando.Parameters.AddWithValue("@fechafin", fechaFin ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@salario", salario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@tarifahora", tarifaHora ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@modopago", modoPago ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@documentourl", documentoUrl ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@descripcionfunciones", descripcionFunciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@observaciones", observaciones ?? (object)DBNull.Value);

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

        public void ActualizarContrato(int contratoId, string usuario, string motivo, string observaciones = null,
            int? cargoId = null, int? tipoSalarioId = null, decimal? contratoSalario = null, string contratoModoPago = null)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("actualizar_contrato");

                comando.Parameters.AddWithValue("@contrato_id", contratoId);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@motivo", motivo);
                comando.Parameters.AddWithValue("@observaciones", observaciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@cargo_id", cargoId ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@tipo_salario_id", tipoSalarioId ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_salario", contratoSalario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_modo_pago", contratoModoPago ?? (object)DBNull.Value);

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
    }
}