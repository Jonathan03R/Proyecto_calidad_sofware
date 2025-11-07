using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using capa_dominio.dto;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class Contratos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Contratos(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL;
        }

        // CREAR CONTRATO

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

        // FINALIZAR CONTRATO
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

        // ACTUALIZAR CONTRATO
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

        // CONSULTAR CONTRATOS POR TRABAJADOR
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
                            ContratoId = reader["ContratoId"] as int?,
                            TrabajadorId = reader["TrabajadorId"] as int?,
                            CargoId = reader["CargoId"] as int?,
                            AreaId = reader["AreaId"] as int?,
                            TipoPensionId = reader["TipoPensionId"] as int?,
                            TipoSalarioId = reader["TipoSalarioId"] as int?,
                            TipoJornadaId = reader["TipoJornadaId"] as int?,
                            FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                            FechaFin = reader["FechaFin"] as DateTime?,
                            Salario = reader["Salario"] as decimal?,
                            TarifaHora = reader["TarifaHora"] as decimal?,
                            ModoPago = reader["ModoPago"]?.ToString(),
                            DocumentoUrl = reader["DocumentoUrl"]?.ToString(),
                            DescripcionFunciones = reader["DescripcionFunciones"]?.ToString(),
                            Observaciones = reader["Observaciones"]?.ToString()
                        };

                        contratos.Add(contrato);
                    }
                }
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return contratos;
        }

        // ============================================================
        // NUEVOS M�TODOS PARA LOS LISTADOS DE REFERENCIA
        // ============================================================

        // Listar trabajadores
        public List<TrabajadorDTO> ObtenerTrabajadores()
        {
            List<TrabajadorDTO> trabajadores = new List<TrabajadorDTO>();
            var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_empleados");

            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    trabajadores.Add(new TrabajadorDTO
                    {
                        TrabajadorId = Convert.ToInt32(reader["TrabajadorId"]),
                        NombreCompleto = reader["NombreCompleto"].ToString()
                    });
                }
            }
            return trabajadores;
        }

        // Listar �reas
        public List<AreaDTO> ObtenerAreas()
        {
            List<AreaDTO> areas = new List<AreaDTO>();
            var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    areas.Add(new AreaDTO
                    {
                        AreaId = Convert.ToInt32(reader["AreaId"]),
                        NombreArea = reader["NombreArea"].ToString()
                    });
                }
            }
            return areas;
        }

        // Listar cargos
        public List<CargoDTO> ObtenerCargos()
        {
            List<CargoDTO> cargos = new List<CargoDTO>();
            var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_cargos");

            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    cargos.Add(new CargoDTO
                    {
                        CargoId = Convert.ToInt32(reader["CargoId"]),
                        NombreCargo = reader["NombreCargo"].ToString()
                    });
                }
            }
            return cargos;
        }

        // Listar tipos de pensi�n
        public List<TipoPensionDTO> ObtenerTiposPension()
        {
            List<TipoPensionDTO> pensiones = new List<TipoPensionDTO>();
            var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_sistema_pensiones");

            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    pensiones.Add(new TipoPensionDTO
                    {
                        TipoPensionId = Convert.ToInt32(reader["TipoPensionId"]),
                        NombreTipo = reader["NombreTipo"].ToString()
                    });
                }
            }
            return pensiones;
        }

        // Listar estados de contrato
        public List<EstadoContratoDTO> ObtenerEstadosContrato()
        {
            return new List<EstadoContratoDTO>
            {
                new EstadoContratoDTO { EstadoId = 1, NombreEstado = "Activo" },
                new EstadoContratoDTO { EstadoId = 2, NombreEstado = "Finalizado" },
                new EstadoContratoDTO { EstadoId = 3, NombreEstado = "Suspendido" },
                new EstadoContratoDTO { EstadoId = 4, NombreEstado = "Inactivo" }
            };
        }
    }
}
