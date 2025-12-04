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
                comando.Parameters.AddWithValue("@modopagoid", contrato.ModoPagoId);
                comando.Parameters.AddWithValue("@fechainicio", contrato.FechaInicio);
                comando.Parameters.AddWithValue("@fechafin", contrato.FechaFin ?? (object)DBNull.Value);

                var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    ? contrato.HorasSemanales.Value
                    : 48;
                comando.Parameters.AddWithValue("@horas_semanales", horas);

                comando.Parameters.AddWithValue("@salario", contrato.Salario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@tarifahora", contrato.TarifaHora ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@documentourl", "");
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

        // =============================================================
        // ACTUALIZAR CONTRATO (Personal.actualizar_contrato)
        // =============================================================
        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.actualizar_contrato");
                comando.Parameters.AddWithValue("@contrato_id", contratoId);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@motivo", motivo);
                comando.Parameters.AddWithValue("@observaciones", contrato.Observaciones ?? (object)DBNull.Value); 
                comando.Parameters.AddWithValue("@cargo_id", (object)contrato.CargoId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@tipo_salario_id", (object)contrato.TipoSalarioId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_salario", contrato.Salario ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@area_id", (object)contrato.AreaId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@tipo_pension_id", (object)contrato.TipoPensionId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@modo_pago_id", (object)contrato.ModoPagoId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_fecha_inicio", contrato.FechaInicio); 
                comando.Parameters.AddWithValue("@contrato_fecha_fin", contrato.FechaFin ?? (object)DBNull.Value);
                int? horas = null;
                if (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                    horas = contrato.HorasSemanales.Value;
                comando.Parameters.AddWithValue("@contrato_horas_semanales", (object)horas ?? DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_tarifa_hora", contrato.TarifaHora ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_documento_url", contrato.DocumentoUrl ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_descripcion_funciones", contrato.DescripcionFunciones ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@contrato_observaciones", contrato.Observaciones ?? (object)DBNull.Value);

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
                _accesoSQL.AbrirConexion();
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
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return contratos;
        }

        // =============================================================
        // LISTAR CONTRATOS ACTIVOS (para tabla principal)
        // =============================================================
        public List<ContratoDTO> ListarConContratoActivo()
        {
            var lista = new List<ContratoDTO>();

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_contratos_activos_listar");

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ContratoDTO
                        {
                            // Claves
                            ContratoId = dr.GetInt32(dr.GetOrdinal("contrato_id")),
                            TrabajadorId = dr["trabajador_id"] as int?,

                            // Persona
                            EmpleadoNombre = dr["persona_nombre"] as string,
                            Documento = dr["persona_identificacion"] as string,

                            // Cargo / salario / estado
                            CargoId = dr["cargo_id"] as int?,
                            CargoNombre = dr["cargo_nombre"] as string,
                            TipoSalarioId = dr["tipo_salario_id"] as int?,
                            EstadoContratoNombre = dr["estado_contrato_nombre"] as string,

                            // Contrato
                            AreaId = dr["area_id"] as int?,
                            TipoPensionId = dr["tipo_pension_id"] as int?,

                            FechaInicio = dr["contrato_fecha_inicio"] == DBNull.Value
                                          ? DateTime.MinValue
                                          : (DateTime)dr["contrato_fecha_inicio"],

                            FechaFin = dr["contrato_fecha_fin"] as DateTime?,

                            Salario = dr["contrato_salario"] as decimal?,
                            Observaciones = dr["contrato_observaciones"] as string,

                            TarifaHora = dr["contrato_tarifa_hora"] as decimal?,
                            HorasSemanales = dr["contrato_horas_semanales"] as int?,
                            DescripcionFunciones = dr["contrato_descripcion_funciones"] as string,

                            ModoPagoId = (int)(dr.IsDBNull(dr.GetOrdinal("modo_pago_id"))
                            ? (int?)null
                            : dr.GetInt32(dr.GetOrdinal("modo_pago_id"))),

                            ModoPagoNombre = dr["modo_pago_nombre"] as string,

                            DocumentoUrl = dr["contrato_documento_url"] as string
                        });
                    }
                }
            }
            catch
            {
                throw;
            }

            return lista;
        }

        public List<ContratoDTO> ListarSinContratoActivo()
        {
            var lista = new List<ContratoDTO>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_obtener_personas_sin_contrato_activo");

                using (var dr = cmd.ExecuteReader())
                {
                    int iTrabId = dr.GetOrdinal("trabajador_id");
                    int iAp = dr.GetOrdinal("persona_apellido");
                    int iNom = dr.GetOrdinal("persona_nombre");
                    int iDoc = dr.GetOrdinal("persona_identificacion");
                    int iEstado = dr.GetOrdinal("EstadoContrato");

                    while (dr.Read())
                    {
                        string ap = dr.IsDBNull(iAp) ? "" : dr.GetString(iAp);
                        string no = dr.IsDBNull(iNom) ? "" : dr.GetString(iNom);

                        lista.Add(new ContratoDTO
                        {
                            TrabajadorId = dr.IsDBNull(iTrabId) ? (int?)null : dr.GetInt32(iTrabId),
                            EmpleadoNombre = (ap + " " + no).Trim(),
                            Documento = dr.IsDBNull(iDoc) ? "" : dr.GetString(iDoc),
                            EstadoContratoNombre = dr.IsDBNull(iEstado) ? "" : dr.GetString(iEstado)
                        });
                    }
                }
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return lista;
        }

        public ResumenContratosDto ObtenerResumenContratos()
        {
            var resumen = new ResumenContratosDto();

            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_contratos_resumen");

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        resumen.TotalContratos = dr.IsDBNull(dr.GetOrdinal("TotalContratos"))
                            ? 0 : dr.GetInt32(dr.GetOrdinal("TotalContratos"));

                        resumen.ContratosActivos = dr.IsDBNull(dr.GetOrdinal("ContratosActivos"))
                            ? 0 : dr.GetInt32(dr.GetOrdinal("ContratosActivos"));

                        resumen.PorVencer30 = dr.IsDBNull(dr.GetOrdinal("PorVencer30"))
                            ? 0 : dr.GetInt32(dr.GetOrdinal("PorVencer30"));

                        resumen.AlertasLegales = dr.IsDBNull(dr.GetOrdinal("AlertasLegales"))
                            ? 0 : dr.GetInt32(dr.GetOrdinal("AlertasLegales"));
                    }
                }
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return resumen;
        }

        public List<ContratoPorPeriodoDTO> ListarContratosPorPeriodo(int periodoId)
        {
            if (periodoId <= 0)
                throw new ArgumentException("Periodo inválido.");

            var lista = new List<ContratoPorPeriodoDTO>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_listar_contratos_por_periodo"
                );

                comando.Parameters.AddWithValue("@periodo_id", periodoId);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dto = new ContratoPorPeriodoDTO
                        {
                            ContratoId = reader.GetInt32(reader.GetOrdinal("contrato_id")),
                            TrabajadorId = reader.IsDBNull(reader.GetOrdinal("trabajador_id"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            TrabajadorCodigo = reader["trabajador_codigo"]?.ToString(),
                            PersonaNombre = reader["persona_nombre"]?.ToString(),
                            PersonaApellido = reader["persona_apellido"]?.ToString(),
                            ContratoSalario = reader.IsDBNull(reader.GetOrdinal("contrato_salario"))
                                ? (decimal?)null
                                : reader.GetDecimal(reader.GetOrdinal("contrato_salario")),
                            TipoPensionId = reader.IsDBNull(reader.GetOrdinal("tipo_pension_id"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("tipo_pension_id")),
                            TieneAsignacionFamiliar =
                                reader.GetInt32(reader.GetOrdinal("tiene_asignacion_familiar")) == 0,
                            CargoId = reader.IsDBNull(reader.GetOrdinal("cargo_id"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("cargo_id")),
                            CargoNombre = reader["cargo_nombre"]?.ToString(),
                            AreaId = reader.IsDBNull(reader.GetOrdinal("area_id"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("area_id")),
                            AreaNombre = reader["area_nombre"]?.ToString(),
                            EstadoContratoId = reader.GetInt32(reader.GetOrdinal("estado_contrato_id")),
                            EstadoContratoNombre = reader["estado_contrato_nombre"]?.ToString(),
                            PeriodoId = reader.GetInt32(reader.GetOrdinal("periodo_id")),
                            PeriodoNombre = reader["periodo_nombre"]?.ToString(),
                            PeriodoFechaInicio = reader.GetDateTime(reader.GetOrdinal("periodo_fecha_inicio")),
                            PeriodoFechaFin = reader.GetDateTime(reader.GetOrdinal("periodo_fecha_fin")),
                            Procesado = reader.GetInt32(reader.GetOrdinal("procesado")) == 1
                        };

                        lista.Add(dto);
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar contratos por periodo.", ex);
            }
        }

                using (var dr = cmd.ExecuteReader())
                {
                    int iContratoId = dr.GetOrdinal("contrato_id");
                    int iTrabajadorId = dr.GetOrdinal("trabajador_id");
                    int iPersonaNombre = dr.GetOrdinal("persona_nombre");
                    int iDocumento = dr.GetOrdinal("persona_identificacion");
                    int iCargoId = dr.GetOrdinal("cargo_id");
                    int iCargoNombre = dr.GetOrdinal("cargo_nombre");
                    int iTipoSalarioId = dr.GetOrdinal("tipo_salario_id");
                    int iEstadoContratoNom = dr.GetOrdinal("estado_contrato_nombre");
                    int iAreaId = dr.GetOrdinal("area_id");
                    int iTipoPensionId = dr.GetOrdinal("tipo_pension_id");
                    int iTipoJornadaId = dr.GetOrdinal("tipo_jornada_id");
                    int iFechaInicio = dr.GetOrdinal("contrato_fecha_inicio");
                    int iFechaFin = dr.GetOrdinal("contrato_fecha_fin");
                    int iSalario = dr.GetOrdinal("contrato_salario");
                    int iModoPago = dr.GetOrdinal("modo_pago_nombre");
                    int iObs = dr.GetOrdinal("contrato_observaciones");
                    int iTarifaHora = dr.GetOrdinal("contrato_tarifa_hora");
                    int iHorasSem = dr.GetOrdinal("contrato_horas_semanales");
                    int iDescFunciones = dr.GetOrdinal("contrato_descripcion_funciones");
                    int iDocUrl = dr.GetOrdinal("contrato_documento_url");

                    while (dr.Read())
                    {
                        lista.Add(new ContratoDTO
                        {
                            ContratoId = dr.GetInt32(iContratoId),
                            TrabajadorId = dr.IsDBNull(iTrabajadorId) ? (int?)null : dr.GetInt32(iTrabajadorId),

                            EmpleadoNombre = dr.IsDBNull(iPersonaNombre) ? "" : dr.GetString(iPersonaNombre),
                            Documento = dr.IsDBNull(iDocumento) ? "" : dr.GetString(iDocumento),

                            CargoId = dr.IsDBNull(iCargoId) ? (int?)null : dr.GetInt32(iCargoId),
                            CargoNombre = dr.IsDBNull(iCargoNombre) ? "" : dr.GetString(iCargoNombre),
                            TipoSalarioId = dr.IsDBNull(iTipoSalarioId) ? (int?)null : dr.GetInt32(iTipoSalarioId),
                            EstadoContratoNombre = dr.IsDBNull(iEstadoContratoNom) ? "" : dr.GetString(iEstadoContratoNom),

                            AreaId = dr.IsDBNull(iAreaId) ? (int?)null : dr.GetInt32(iAreaId),
                            TipoPensionId = dr.IsDBNull(iTipoPensionId) ? (int?)null : dr.GetInt32(iTipoPensionId),
                            TipoJornadaId = dr.IsDBNull(iTipoJornadaId) ? (int?)null : dr.GetInt32(iTipoJornadaId),

                            FechaInicio = dr.IsDBNull(iFechaInicio) ? DateTime.MinValue : dr.GetDateTime(iFechaInicio),
                            FechaFin = dr.IsDBNull(iFechaFin) ? (DateTime?)null : dr.GetDateTime(iFechaFin),

                            Salario = dr.IsDBNull(iSalario) ? (decimal?)null : dr.GetDecimal(iSalario),
                            ModoPago = dr.IsDBNull(iModoPago) ? null : dr.GetString(iModoPago),
                            Observaciones = dr.IsDBNull(iObs) ? null : dr.GetString(iObs),

                            TarifaHora = dr.IsDBNull(iTarifaHora) ? (decimal?)null : dr.GetDecimal(iTarifaHora),
                            HorasSemanales = dr.IsDBNull(iHorasSem) ? (int?)null : dr.GetInt32(iHorasSem),
                            DescripcionFunciones = dr.IsDBNull(iDescFunciones) ? null : dr.GetString(iDescFunciones),
                            DocumentoUrl = dr.IsDBNull(iDocUrl) ? null : dr.GetString(iDocUrl)
                        });
                    }
                }
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return lista;
        }
    }
}
