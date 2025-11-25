using capa_dominio.dto;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class DetallesNominaRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        // ✅ Recibe la instancia desde la capa de aplicación
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
                //cmd.Parameters.AddWithValue("@trabajador_id", detalle.TrabajadorId);
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
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CREACION);
            }

        }

        public List<NominasProcesadasDTO> ListarDetallesNominasProcesadas(
            int? trabajadorId = null,
            int? nominaId = null,
            int? periodoId = null,
            string estadoNomina = null)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Listando detalles de nóminas procesadas - Trabajador: {trabajadorId}, Nómina: {nominaId}, Periodo: {periodoId}, Estado: {estadoNomina}");

            List<NominasProcesadasDTO> listaDetalles = new List<NominasProcesadasDTO>();

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_listar_detalle_nominas_procesadas");

                // Parámetros opcionales
                cmd.Parameters.AddWithValue("@trabajador_id", (object)trabajadorId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_id", (object)nominaId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@periodo_id", (object)periodoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado_nomina", (object)estadoNomina ?? DBNull.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NominasProcesadasDTO detalle = new NominasProcesadasDTO
                        {
                            // Detalle de la nómina
                            DetalleNominaId = reader.GetInt32(reader.GetOrdinal("detalle_nomina_id")),
                            NominaId = reader.GetInt32(reader.GetOrdinal("nomina_id")),

                            // Información del trabajador
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            ContratoId = reader.GetInt32(reader.GetOrdinal("contrato_id")),
                            PeriodoId = reader.GetInt32(reader.GetOrdinal("periodo_id")),
                            Nombre = reader.GetString(reader.GetOrdinal("persona_nombre")),
                            Apellidos = reader.GetString(reader.GetOrdinal("persona_apellido")),
                            NominaEstado = reader.GetString(reader.GetOrdinal("nomina_estado")),
                            EstadoContratoNombre = reader.GetString(reader.GetOrdinal("estado_contrato_nombre")),

                            // Ingresos
                            SueldoBasico = reader.GetDecimal(reader.GetOrdinal("sueldo_basico")),
                            AsignacionFamiliar = reader.GetDecimal(reader.GetOrdinal("asignacion_familiar")),
                            HorasExtras = reader.GetDecimal(reader.GetOrdinal("horas_extras")),
                            BonosRegulares = reader.GetDecimal(reader.GetOrdinal("bonos_regulares")),
                            OtrosIngresos = reader.GetDecimal(reader.GetOrdinal("otros_ingresos")),
                            RemuneracionBruta = reader.GetDecimal(reader.GetOrdinal("remuneracion_bruta")),
                            TotalIngresos = reader.GetDecimal(reader.GetOrdinal("total_ingresos")),

                            // Descuentos por pensiones
                            SistemaPensionAplicado = reader.IsDBNull(reader.GetOrdinal("sistema_pension_aplicado"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("sistema_pension_aplicado")),
                            AporteEssalud = reader.GetDecimal(reader.GetOrdinal("aporte_essalud")),
                            AporteOnp = reader.GetDecimal(reader.GetOrdinal("aporte_onp")),
                            DescuentoAfp = reader.GetDecimal(reader.GetOrdinal("descuento_afp")),

                            // Impuesto a la renta
                            RemuneracionAcumuladaAnual = reader.GetDecimal(reader.GetOrdinal("remuneracion_acumulada_anual")),
                            BaseImponibleAnual = reader.GetDecimal(reader.GetOrdinal("base_imponible_anual")),
                            ImpuestoRentaAnual = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_anual")),
                            ImpuestoRentaMensual = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_mensual")),
                            UitValor = reader.GetDecimal(reader.GetOrdinal("uit_valor")),
                            Deduccion7Uit = reader.GetDecimal(reader.GetOrdinal("deduccion_7uit")),

                            // Otros descuentos
                            DescuentoTardanzas = reader.GetDecimal(reader.GetOrdinal("descuento_tardanzas")),
                            DescuentoFaltas = reader.GetDecimal(reader.GetOrdinal("descuento_faltas")),
                            DescuentoAdelantos = reader.GetDecimal(reader.GetOrdinal("descuento_adelantos")),
                            OtrosDescuentos = reader.GetDecimal(reader.GetOrdinal("otros_descuentos")),

                            // Totales
                            TotalDescuentos = reader.GetDecimal(reader.GetOrdinal("total_descuentos")),
                            NetoPagar = reader.GetDecimal(reader.GetOrdinal("neto_pagar"))
                        };

                        listaDetalles.Add(detalle);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Se encontraron {listaDetalles.Count} detalles de nóminas procesadas");
                return listaDetalles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al listar detalles de nóminas procesadas: {ex.Message}");
                throw new ExcepcionNomina("Error al obtener los detalles de las nóminas procesadas");
            }
        }
    }
}
