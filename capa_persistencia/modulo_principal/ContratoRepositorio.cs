using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
        public List<ContratoDTO> ListarConContratoActivo()
        {
            var lista = new List<ContratoDTO>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_contratos_activos_listar");

                using (var dr = cmd.ExecuteReader())
                {
                    int iPersonaNombre = dr.GetOrdinal("persona_nombre");
                    int iDocumento = dr.GetOrdinal("persona_identificacion");
                    int iCargoNombre = dr.GetOrdinal("cargo_nombre");
                    int iEstadoContratoNombre = dr.GetOrdinal("estado_contrato_nombre");
                    int iFechaInicio = dr.GetOrdinal("contrato_fecha_inicio");
                    int iFechaFin = dr.GetOrdinal("contrato_fecha_fin");

                    while (dr.Read())
                    {
                        lista.Add(new ContratoDTO
                        {
                            EmpleadoNombre = dr.IsDBNull(iPersonaNombre) ? "" : dr.GetString(iPersonaNombre),
                            Documento = dr.IsDBNull(iDocumento) ? "" : dr.GetString(iDocumento),
                            CargoNombre = dr.IsDBNull(iCargoNombre) ? "" : dr.GetString(iCargoNombre),
                            EstadoContratoNombre = dr.IsDBNull(iEstadoContratoNombre) ? "" : dr.GetString(iEstadoContratoNombre),
                            FechaInicio = dr.IsDBNull(iFechaInicio) ? DateTime.MinValue : dr.GetDateTime(iFechaInicio),
                            FechaFin = dr.IsDBNull(iFechaFin) ? (DateTime?)null : dr.GetDateTime(iFechaFin),
                        });
                    }
                }
            }
            finally { _accesoSQL.CerrarConexion(); }

            return lista;
        }







        //    public (List<Contrato> data, int total) ConsultarTodosLosContratos(
        //int? estadoId, int? tipoContratoId, string query, int page, int pageSize)
        //    {
        //        var lista = new List<Contrato>();
        //        int total = 0;

        //        _accesoSQL.AbrirConexion();
        //        try
        //        {
        //            // Usa tu helper para SP; NO es código en la vista, esto es capa de persistencia ✅
        //            var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("proc_listar_contratos");

        //            cmd.Parameters.AddWithValue("@estadoId", (object)estadoId ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("@tipoContratoId", (object)tipoContratoId ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("@query", (object)(query ?? ""));
        //            cmd.Parameters.AddWithValue("@page", page);
        //            cmd.Parameters.AddWithValue("@pageSize", pageSize);

        //            // puedes usar output param o un segundo resultset; aquí te dejo ambas formas:
        //            var pTotal = new SqlParameter("@total", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            cmd.Parameters.Add(pTotal);

        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    var c = new Contrato
        //                    {
        //                        ContratoId = dr.GetInt32(dr.GetOrdinal("contrato_id")),
        //                        ContratoFechaInicio = dr.GetDateTime(dr.GetOrdinal("contrato_fecha_inicio")),
        //                        ContratoFechaFin = dr.IsDBNull(dr.GetOrdinal("contrato_fecha_fin"))
        //                            ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("contrato_fecha_fin")),
        //                        ContratoSalario = dr.IsDBNull(dr.GetOrdinal("contrato_salario"))
        //                            ? 0 : dr.GetDecimal(dr.GetOrdinal("contrato_salario")),
        //                        ContratoTarifaHora = dr.IsDBNull(dr.GetOrdinal("contrato_tarifa_hora"))
        //                            ? 0 : dr.GetDecimal(dr.GetOrdinal("contrato_tarifa_hora")),
        //                        ContratoModoPago = dr["contrato_modo_pago"]?.ToString(),
        //                        ContratoDocumentoUrl = dr["contrato_documento_url"]?.ToString(),
        //                        ContratoDescripcionFunciones = dr["contrato_descripcion_funciones"]?.ToString(),
        //                        ContratoObservaciones = dr["contrato_observaciones"]?.ToString(),
        //                        EstadoiId = dr.GetInt32(dr.GetOrdinal("estado_contrato_id")),
        //                        Trabajador = new Trabajador
        //                        {
        //                            // Ajusta nombres a tus columnas reales del SP
        //                            Nombres = dr["empleado_nombre_completo"]?.ToString(),

        //                            Identificacion = dr["empleado_documento"]?.ToString(),
        //                        },
        //                        Cargo = new Cargo { CargoNombre = dr["cargo_nombre"]?.ToString() },
        //                        TipoSalario = new TipoSalario { TipoSalarioNombre = dr["tipo_salario_nombre"]?.ToString() }
        //                    };

        //                    lista.Add(c);
        //                }

        //                // si tu SP devuelve el total en un segundo resultset:
        //                if (dr.NextResult() && dr.Read())
        //                {
        //                    total = dr.GetInt32(dr.GetOrdinal("total"));
        //                }
        //            }

        //            // si usaste parámetro de salida:
        //            if (total == 0 && pTotal.Value != DBNull.Value)
        //                total = Convert.ToInt32(pTotal.Value);

        //            return (lista, total);
        //        }
        //        finally
        //        {
        //            _accesoSQL.CerrarConexion();
        //        }

        //// ============================================================
        //// NUEVOS M�TODOS PARA LOS LISTADOS DE REFERENCIA
        //// ============================================================

        ////// Listar trabajadores
        //public List<TrabajadorDTO> ObtenerTrabajadores()
        //{
        //    List<TrabajadorDTO> trabajadores = new List<TrabajadorDTO>();
        //    var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_empleados");

        //    using (var reader = comando.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            trabajadores.Add(new TrabajadorDTO
        //            {
        //                TrabajadorId = Convert.ToInt32(reader["TrabajadorId"]),
        //                NombreCompleto = reader["NombreCompleto"].ToString()
        //            });
        //        }
        //    }
        //    return trabajadores;
        //}

        //// Listar �reas
        //public List<AreaDTO> ObtenerAreas()
        //{
        //    List<AreaDTO> areas = new List<AreaDTO>();
        //    var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

        //    using (var reader = comando.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            areas.Add(new AreaDTO
        //            {
        //                AreaId = Convert.ToInt32(reader["AreaId"]),
        //                NombreArea = reader["NombreArea"].ToString()
        //            });
        //        }
        //    }
        //    return areas;
        //}

        //// Listar cargos
        //public List<CargoDTO> ObtenerCargos()
        //{
        //    List<CargoDTO> cargos = new List<CargoDTO>();
        //    var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_cargos");

        //    using (var reader = comando.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            cargos.Add(new CargoDTO
        //            {
        //                CargoId = Convert.ToInt32(reader["CargoId"]),
        //                NombreCargo = reader["NombreCargo"].ToString()
        //            });
        //        }
        //    }
        //    return cargos;
        //}

        //// Listar tipos de pensi�n
        //public List<TipoPensionDTO> ObtenerTiposPension()
        //{
        //    List<TipoPensionDTO> pensiones = new List<TipoPensionDTO>();
        //    var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_sistema_pensiones");

        //    using (var reader = comando.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            pensiones.Add(new TipoPensionDTO
        //            {
        //                TipoPensionId = Convert.ToInt32(reader["TipoPensionId"]),
        //                NombreTipo = reader["NombreTipo"].ToString()
        //            });
        //        }
        //    }
        //    return pensiones;
        //}

        //// Listar estados de contrato
        //public List<EstadoContratoDTO> ObtenerEstadosContrato()
        //{
        //    return new List<EstadoContratoDTO>
        //    {
        //        new EstadoContratoDTO { EstadoId = 1, NombreEstado = "Activo" },
        //        new EstadoContratoDTO { EstadoId = 2, NombreEstado = "Finalizado" },
        //        new EstadoContratoDTO { EstadoId = 3, NombreEstado = "Suspendido" },
        //        new EstadoContratoDTO { EstadoId = 4, NombreEstado = "Inactivo" }
        //    };
        //}
    }
}
