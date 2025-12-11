using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.helpers;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class DetallesNominaRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public DetallesNominaRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public int InsertarDetalleNomina(DetalleNominaDTO detalle)
        {

            System.Diagnostics.Debug.WriteLine(
                $"Insertando detalle de nómina para trabajador ID: {detalle.ContratoId} en nómina ID: {detalle.NominaId}");
            try
            {

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_detalle_nomina");

                cmd.Parameters.AddWithValue("@nomina_id", detalle.NominaId);
                cmd.Parameters.AddWithValue("@contrato_id", detalle.ContratoId);
                cmd.Parameters.AddWithValue("@remuneracion_bruta", detalle.RemuneracionBruta);
                cmd.Parameters.AddWithValue("@sueldo_basico", detalle.SueldoBasico);
                cmd.Parameters.AddWithValue("@asignacion_familiar", detalle.AsignacionFamiliar);
                cmd.Parameters.AddWithValue("@horas_extras", detalle.HorasExtras);
                cmd.Parameters.AddWithValue("@bonos_regulares", detalle.BonosRegulares);
                cmd.Parameters.AddWithValue("@otros_ingresos", detalle.OtrosIngresos);
                cmd.Parameters.AddWithValue("@sistema_pension_aplicado", (object)detalle.SistemaPensionAplicado);
                cmd.Parameters.AddWithValue("@aporte_essalud", detalle.AporteEssalud);
                cmd.Parameters.AddWithValue("@aporte_onp", detalle.AporteOnp);
                cmd.Parameters.AddWithValue("@descuento_afp", detalle.DescuentoAfp);
                cmd.Parameters.AddWithValue("@remuneracion_acumulada_anual", detalle.RemuneracionAcumuladaAnual);
                cmd.Parameters.AddWithValue("@base_imponible_anual", detalle.BaseImponibleAnual);
                cmd.Parameters.AddWithValue("@impuesto_renta_anual", detalle.ImpuestoRentaAnual);
                cmd.Parameters.AddWithValue("@impuesto_renta_mensual", detalle.ImpuestoRentaMensual);
                cmd.Parameters.AddWithValue("@uit_valor", detalle.UitValor);
                cmd.Parameters.AddWithValue("@deduccion_7uit", detalle.Deduccion7Uit);
                cmd.Parameters.AddWithValue("@descuento_tardanzas", detalle.DescuentoTardanzas);
                cmd.Parameters.AddWithValue("@descuento_faltas", detalle.DescuentoFaltas);
                cmd.Parameters.AddWithValue("@descuento_adelantos", detalle.DescuentoAdelantos);
                cmd.Parameters.AddWithValue("@otros_descuentos", detalle.OtrosDescuentos);
                cmd.Parameters.AddWithValue("@total_ingresos", detalle.TotalIngresos); ///
                cmd.Parameters.AddWithValue("@total_descuentos", detalle.TotalDescuentos);
                cmd.Parameters.AddWithValue("@neto_pagar", detalle.NetoPagar); ////
                cmd.Parameters.AddWithValue("@tiene_errores", detalle.TieneErrores);
                cmd.Parameters.AddWithValue("@mensaje_error", (object)detalle.MensajeError ?? DBNull.Value);

                var pOut = new SqlParameter("@detalle_nomina_id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pOut);

                cmd.ExecuteNonQuery();
                return (int)pOut.Value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new NominaException(NominaException.ERROR_DE_CREACION);
            }

        }

        public List<NominasProcesadasDTO> ListarDetallesNominasProcesadas(
            int? trabajadorId = null,
            int? nominaId = null,
            int? periodoId = null,
            string estadoNomina = null)
        {
            var lista = new List<NominasProcesadasDTO>();

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_listar_detalle_nominas_procesadas");

                cmd.Parameters.AddWithValue("@trabajador_id", (object)trabajadorId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_id", (object)nominaId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@periodo_id", (object)periodoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado_nomina", (object)estadoNomina ?? DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var dto = new NominasProcesadasDTO
                        {
                            DetalleNominaId = dr.GetInt32(dr.GetOrdinal("detalle_nomina_id")),
                            NominaId = dr.GetInt32(dr.GetOrdinal("nomina_id")),
                            TrabajadorId = dr.GetInt32(dr.GetOrdinal("trabajador_id")),
                            ContratoId = dr.GetInt32(dr.GetOrdinal("contrato_id")),
                            PeriodoId = dr.GetInt32(dr.GetOrdinal("periodo_id")),
                            Nombre = DataReaderHelper.GetString(dr, "persona_nombre"),
                            Apellidos = DataReaderHelper.GetString(dr, "persona_apellido"),
                            NominaEstado = DataReaderHelper.GetString(dr, "nomina_estado"),
                            EstadoContratoNombre = DataReaderHelper.GetString(dr, "estado_contrato_nombre"),
                            SueldoBasico = DataReaderHelper.GetDecimal(dr, "sueldo_basico") ?? 0,
                            AsignacionFamiliar = DataReaderHelper.GetDecimal(dr, "asignacion_familiar") ?? 0,
                            HorasExtras = DataReaderHelper.GetDecimal(dr, "horas_extras") ?? 0,
                            BonosRegulares = DataReaderHelper.GetDecimal(dr, "bonos_regulares") ?? 0,
                            OtrosIngresos = DataReaderHelper.GetDecimal(dr, "otros_ingresos") ?? 0,
                            RemuneracionBruta = DataReaderHelper.GetDecimal(dr, "remuneracion_bruta") ?? 0,
                            TotalIngresos = DataReaderHelper.GetDecimal(dr, "total_ingresos") ?? 0,
                            SistemaPensionAplicado = DataReaderHelper.GetStringNull(dr, "sistema_pension_aplicado"),
                            AporteEssalud = DataReaderHelper.GetDecimal(dr, "aporte_essalud") ?? 0,
                            AporteOnp = DataReaderHelper.GetDecimal(dr, "aporte_onp") ?? 0,
                            DescuentoAfp = DataReaderHelper.GetDecimal(dr, "descuento_afp") ?? 0,
                            RemuneracionAcumuladaAnual = DataReaderHelper.GetDecimal(dr, "remuneracion_acumulada_anual") ?? 0,
                            BaseImponibleAnual = DataReaderHelper.GetDecimal(dr, "base_imponible_anual") ?? 0,
                            ImpuestoRentaAnual = DataReaderHelper.GetDecimal(dr, "impuesto_renta_anual") ?? 0,
                            ImpuestoRentaMensual = DataReaderHelper.GetDecimal(dr, "impuesto_renta_mensual") ?? 0,
                            UitValor = DataReaderHelper.GetDecimal(dr, "uit_valor") ?? 0,
                            Deduccion7Uit = DataReaderHelper.GetDecimal(dr, "deduccion_7uit") ?? 0,
                            DescuentoTardanzas = DataReaderHelper.GetDecimal(dr, "descuento_tardanzas") ?? 0,
                            DescuentoFaltas = DataReaderHelper.GetDecimal(dr, "descuento_faltas") ?? 0,
                            DescuentoAdelantos = DataReaderHelper.GetDecimal(dr, "descuento_adelantos") ?? 0,
                            OtrosDescuentos = DataReaderHelper.GetDecimal(dr, "otros_descuentos") ?? 0,
                            TotalDescuentos = DataReaderHelper.GetDecimal(dr, "total_descuentos") ?? 0,
                            NetoPagar = DataReaderHelper.GetDecimal(dr, "neto_pagar") ?? 0
                        };

                        lista.Add(dto);
                    }
                }

                return lista;
            }
            catch
            {
                throw new NominaException("Error al obtener los detalles de las nóminas procesadas");
            }
        }


        public bool ExisteDetalleParaContrato(int nominaId, int contratoId)
        {
            string sql = @"
                select count(*) 
                from nomina.detalle_nomina
                where nomina_id = @nomina_id
                and contrato_id = @contrato_id;
            ";

            var cmd = _accesoSQL.ObtenerComandoSQL(sql);
            cmd.Parameters.AddWithValue("@nomina_id", nominaId);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }


        public List<DetalleNomina> ListarDetallesPorNomina(int nominaId)
        {
            var lista = new List<DetalleNomina>();

            string sql = @"
                select 
                    contrato_id,
                    remuneracion_bruta,
                    sueldo_basico,
                    asignacion_familiar,
                    horas_extras,
                    bonos_regulares,
                    otros_ingresos,
                    aporte_essalud,
                    aporte_onp,
                    descuento_afp,
                    descuento_tardanzas,
                    descuento_faltas,
                    descuento_adelantos,
                    otros_descuentos,
                    total_ingresos,
                    total_descuentos,
                    neto_pagar
                from nomina.detalle_nomina
                where nomina_id = @nomina_id;
            ";

            var cmd = _accesoSQL.ObtenerComandoSQL(sql);
            cmd.Parameters.AddWithValue("@nomina_id", nominaId);

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var detalle = new DetalleNomina
                    {
                        Contrato = new Contrato { ContratoId = dr.GetInt32(0) },
                        RemuneracionBruta = dr.GetDecimal(1),
                        SueldoBasico = dr.GetDecimal(2),
                        AsignacionFamiliar = dr.GetDecimal(3),
                        HorasExtras = dr.GetDecimal(4),
                        BonosRegulares = dr.GetDecimal(5),
                        OtrosIngresos = dr.GetDecimal(6),
                        AporteEssalud = dr.GetDecimal(7),
                        AporteONP = dr.GetDecimal(8),
                        DescuentoAFP = dr.GetDecimal(9),
                        DescuentoTardanzas = dr.GetDecimal(10),
                        DescuentoFaltas = dr.GetDecimal(11),
                        DescuentoAdelantos = dr.GetDecimal(12),
                        TotalIngresos = dr.GetDecimal(14),
                        TotalDescuentos = dr.GetDecimal(15),
                        NetoPagar = dr.GetDecimal(16)
                    };

                    lista.Add(detalle);
                }
            }

            return lista;
        }


    }
}
