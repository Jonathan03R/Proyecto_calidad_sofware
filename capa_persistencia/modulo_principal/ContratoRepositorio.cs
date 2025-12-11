using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.helpers;
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
            var comando = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.actualizar_contrato");

            comando.Parameters.AddWithValue("@contrato_id", contratoId);
            comando.Parameters.AddWithValue("@usuario", usuario);
            comando.Parameters.AddWithValue("@motivo", motivo);

            comando.Parameters.AddWithValue("@observaciones", (object)contrato.Observaciones ?? DBNull.Value);
            comando.Parameters.AddWithValue("@cargo_id", (object)contrato.CargoId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@tipo_salario_id", (object)contrato.TipoSalarioId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@contrato_salario", (object)contrato.Salario ?? DBNull.Value);
            comando.Parameters.AddWithValue("@area_id", (object)contrato.AreaId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@tipo_pension_id", (object)contrato.TipoPensionId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@modo_pago_id", (object)contrato.ModoPagoId ?? DBNull.Value);

            comando.Parameters.AddWithValue("@contrato_fecha_inicio", contrato.FechaInicio);
            comando.Parameters.AddWithValue("@contrato_fecha_fin", (object)contrato.FechaFin ?? DBNull.Value);

            var horas = (contrato.HorasSemanales.HasValue && contrato.HorasSemanales.Value > 0)
                ? contrato.HorasSemanales.Value
                : (int?)null;

            comando.Parameters.AddWithValue("@contrato_horas_semanales", (object)horas ?? DBNull.Value);
            comando.Parameters.AddWithValue("@contrato_tarifa_hora", (object)contrato.TarifaHora ?? DBNull.Value);

            comando.Parameters.AddWithValue("@contrato_documento_url", (object)contrato.DocumentoUrl ?? DBNull.Value);
            comando.Parameters.AddWithValue("@contrato_descripcion_funciones", (object)contrato.DescripcionFunciones ?? DBNull.Value);
            comando.Parameters.AddWithValue("@contrato_observaciones", (object)contrato.Observaciones ?? DBNull.Value);

            comando.ExecuteNonQuery();
        }


        public List<Contrato> ObtenerContratosPorTrabajador(int trabajadorId)
        {
            var contratos = new List<Contrato>();

            var comando = _accesoSQL.ObtenerComandoDeProcedimiento(
                "proc_obtener_contratos_por_trabajador"
            );

            comando.Parameters.AddWithValue("@trabajador_id", trabajadorId);

            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    var contrato = new Contrato
                    {
                        ContratoId = reader.GetInt32(reader.GetOrdinal("contrato_id")),
                        ContratoFechaInicio = reader.GetDateTime(reader.GetOrdinal("contrato_fecha_inicio")),
                        ContratoFechaFin = DataReaderHelper.GetDate(reader, "contrato_fecha_fin"),
                        ContratoSalario = DataReaderHelper.GetDecimal(reader, "contrato_salario") ?? 0,
                        ContratoTarifaHora = DataReaderHelper.GetDecimal(reader, "contrato_tarifa_hora") ?? 0,
                        ContratoHorasSemanales = DataReaderHelper.GetInt(reader, "contrato_horas_semanales"),
                        ContratoModoPago = reader["contrato_modo_pago"]?.ToString(),
                        ContratoDocumentoUrl = reader["contrato_documento_url"]?.ToString(),
                        ContratoDescripcionFunciones = reader["contrato_descripcion_funciones"]?.ToString(),
                        ContratoObservaciones = reader["contrato_observaciones"]?.ToString(),
                        EstadoiId = reader.GetInt32(reader.GetOrdinal("estado_contrato_id")),
                    };

                    if (DataReaderHelper.GetInt(reader, "tipo_pension_id") != null)
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

            return contratos;
        }



        public List<ContratoDTO> ListarConContratoActivo()
        {
            var lista = new List<ContratoDTO>();

            var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                "Personal.proc_contratos_activos_listar"
            );

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    lista.Add(new ContratoDTO
                    {
                        ContratoId = dr.GetInt32(dr.GetOrdinal("contrato_id")),
                        TrabajadorId = DataReaderHelper.GetInt(dr, "trabajador_id"),
                        EmpleadoNombre = DataReaderHelper.GetString(dr, "persona_nombre"),
                        Documento = DataReaderHelper.GetString(dr, "persona_identificacion"),
                        CargoId = DataReaderHelper.GetInt(dr, "cargo_id"),
                        CargoNombre = DataReaderHelper.GetString(dr, "cargo_nombre"),
                        TipoSalarioId = DataReaderHelper.GetInt(dr, "tipo_salario_id"),
                        EstadoContratoNombre = DataReaderHelper.GetString(dr, "estado_contrato_nombre"),
                        AreaId = DataReaderHelper.GetInt(dr, "area_id"),
                        TipoPensionId = DataReaderHelper.GetInt(dr, "tipo_pension_id"),
                        FechaInicio = DataReaderHelper.GetDate(dr, "contrato_fecha_inicio")
                                     ?? DateTime.MinValue,
                        FechaFin = DataReaderHelper.GetDate(dr, "contrato_fecha_fin"),
                        Salario = DataReaderHelper.GetDecimal(dr, "contrato_salario"),
                        Observaciones = DataReaderHelper.GetStringNull(dr, "contrato_observaciones"),
                        TarifaHora = DataReaderHelper.GetDecimal(dr, "contrato_tarifa_hora"),
                        HorasSemanales = DataReaderHelper.GetInt(dr, "contrato_horas_semanales"),
                        DescripcionFunciones = DataReaderHelper.GetStringNull(dr, "contrato_descripcion_funciones"),
                        ModoPagoId = DataReaderHelper.GetInt(dr, "modo_pago_id"),
                        ModoPagoNombre = DataReaderHelper.GetString(dr, "modo_pago_nombre"),
                        DocumentoUrl = DataReaderHelper.GetStringNull(dr, "contrato_documento_url")
                    });
                }
            }

            return lista;
        }

        public List<ContratoDTO> ListarSinContratoActivo()
        {
            var lista = new List<ContratoDTO>();

            var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                "Personal.proc_obtener_personas_sin_contrato_activo"
            );

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var apellido = DataReaderHelper.GetString(dr, "persona_apellido");
                    var nombre = DataReaderHelper.GetString(dr, "persona_nombre");

                    lista.Add(new ContratoDTO
                    {
                        TrabajadorId = DataReaderHelper.GetInt(dr, "trabajador_id"),
                        EmpleadoNombre = $"{apellido} {nombre}".Trim(),
                        Documento = DataReaderHelper.GetString(dr, "persona_identificacion"),
                        EstadoContratoNombre = DataReaderHelper.GetString(dr, "EstadoContrato")
                    });
                }
            }

            return lista;
        }

        public ResumenContratosDto ObtenerResumenContratos()
        {
            var resumen = new ResumenContratosDto();

            var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_contratos_resumen");

            using (var dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    resumen.TotalContratos = DataReaderHelper.GetInt(dr, "TotalContratos") ?? 0;
                    resumen.ContratosActivos = DataReaderHelper.GetInt(dr, "ContratosActivos") ?? 0;
                    resumen.PorVencer30 = DataReaderHelper.GetInt(dr, "PorVencer30") ?? 0;
                    resumen.AlertasLegales = DataReaderHelper.GetInt(dr, "AlertasLegales") ?? 0;
                }
            }

            return resumen;
        }

        public List<ContratoPorPeriodoDTO> ListarContratosPorPeriodo(int periodoId)
        {
        if (periodoId <= 0)
            throw new ArgumentException("Periodo inválido.");

        var lista = new List<ContratoPorPeriodoDTO>();

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
                    TrabajadorId = DataReaderHelper.GetInt(reader, "trabajador_id"),
                    TrabajadorCodigo = reader["trabajador_codigo"]?.ToString(),
                    PersonaNombre = reader["persona_nombre"]?.ToString(),
                    PersonaApellido = reader["persona_apellido"]?.ToString(),

                    ContratoSalario = DataReaderHelper.GetDecimal(reader, "contrato_salario"),
                    TipoPensionId = DataReaderHelper.GetInt(reader, "tipo_pension_id"),

                    TieneAsignacionFamiliar =
                        reader.GetInt32(reader.GetOrdinal("tiene_asignacion_familiar")) == 0,

                    CargoId = DataReaderHelper.GetInt(reader, "cargo_id"),
                    CargoNombre = reader["cargo_nombre"]?.ToString(),

                    AreaId = DataReaderHelper.GetInt(reader, "area_id"),
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
 }
}
