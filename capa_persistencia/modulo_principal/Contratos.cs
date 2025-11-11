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
        // MÉTODOS PARA LOS LISTADOS DE REFERENCIA - ACTUALIZADOS ✅
        // ============================================================

        // Listar trabajadores
        public List<TrabajadorDTO> ObtenerTrabajadores()
        {
            List<TrabajadorDTO> trabajadores = new List<TrabajadorDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado con nombres reales de las tablas
                string query = @"
                    SELECT 
                        t.trabajador_id AS TrabajadorId,
                        CONCAT(p.persona_nombre, ' ', p.persona_apellido) AS NombreCompleto
                    FROM Personal.trabajadores t
                    INNER JOIN Personal.personas p ON t.persona_id = p.persona_id
                    WHERE t.trabajador_estado = 'A'
                    ORDER BY p.persona_apellido, p.persona_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener trabajadores: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return trabajadores;
        }

        // Listar sedes (áreas)
        public List<AreaDTO> ObtenerAreas()
        {
            List<AreaDTO> areas = new List<AreaDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado - Usando tabla 'sedes' como áreas
                string query = @"
                    SELECT 
                        sede_id AS AreaId,
                        sede_nombre AS NombreArea
                    FROM Personal.sedes
                    WHERE sede_estado = 'A'
                    ORDER BY sede_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener sedes: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return areas;
        }

        // Listar cargos
        public List<CargoDTO> ObtenerCargos()
        {
            List<CargoDTO> cargos = new List<CargoDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado con nombres reales de las columnas
                string query = @"
                    SELECT 
                        cargo_id AS CargoId,
                        cargo_nombre AS NombreCargo
                    FROM Personal.cargos
                    WHERE cargo_estado = 'A'
                    ORDER BY cargo_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cargos: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return cargos;
        }

        // Listar tipos de pensión
        public List<TipoPensionDTO> ObtenerTiposPension()
        {
            List<TipoPensionDTO> pensiones = new List<TipoPensionDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ ACTUALIZADO - Usando el nombre correcto de la tabla
                string query = @"
                    SELECT 
                        tipo_pension_id AS TipoPensionId,
                        tipo_pension_nombre AS NombreTipo
                    FROM Personal.tipos_pensiones
                    WHERE tipo_pension_estado = 'A'
                    ORDER BY tipo_pension_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tipos de pensión: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return pensiones;
        }

        // Listar tipos de salarios
        public List<TipoSalarioDTO> ObtenerTiposSalario()
        {
            List<TipoSalarioDTO> tiposSalario = new List<TipoSalarioDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado con la tabla correcta
                string query = @"
                    SELECT 
                        tipo_salario_id AS TipoSalarioId,
                        tipo_salario_nombre AS NombreTipo
                    FROM Personal.tipos_salarios
                    WHERE tipo_salario_estado = 'A'
                    ORDER BY tipo_salario_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tiposSalario.Add(new TipoSalarioDTO
                        {
                            TipoSalarioId = Convert.ToInt32(reader["TipoSalarioId"]),
                            NombreTipo = reader["NombreTipo"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tipos de salario: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return tiposSalario;
        }

        // Listar tipos de jornadas
        public List<TipoJornadaDTO> ObtenerTiposJornada()
        {
            List<TipoJornadaDTO> tiposJornada = new List<TipoJornadaDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado con la tabla correcta
                string query = @"
                    SELECT 
                        tipo_jornada_id AS TipoJornadaId,
                        tipo_jornada_nombre AS NombreTipo
                    FROM Personal.tipos_jornadas
                    WHERE tipo_jornada_estado = 'A'
                    ORDER BY tipo_jornada_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tiposJornada.Add(new TipoJornadaDTO
                        {
                            TipoJornadaId = Convert.ToInt32(reader["TipoJornadaId"]),
                            NombreTipo = reader["NombreTipo"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tipos de jornada: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return tiposJornada;
        }

        // Listar estados de contrato
        public List<EstadoContratoDTO> ObtenerEstadosContrato()
        {
            List<EstadoContratoDTO> estados = new List<EstadoContratoDTO>();

            try
            {
                _accesoSQL.AbrirConexion();

                // ✅ Actualizado - Leyendo desde la tabla real
                string query = @"
                    SELECT 
                        estado_contrato_id AS EstadoId,
                        estado_contrato_nombre AS NombreEstado
                    FROM Personal.estado_contratos
                    ORDER BY estado_contrato_nombre";

                var comando = _accesoSQL.ObtenerComandoSQL(query);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        estados.Add(new EstadoContratoDTO
                        {
                            EstadoId = Convert.ToInt32(reader["EstadoId"]),
                            NombreEstado = reader["NombreEstado"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estados de contrato: {ex.Message}", ex);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return estados;
        }
    }
}