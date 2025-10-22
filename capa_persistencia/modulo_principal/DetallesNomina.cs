using capa_dominio.dto;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace capa_persistencia.modulo_principal
{
    public class DetallesNomina
    {
        private readonly AccesoSQLServer _accesoSQL;

        public DetallesNomina() { _accesoSQL = new AccesoSQLServer(); }

        public int InsertarDetalleNomina(DetalleNominaDTO detalle)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_detalle_nomina_por_trabajador");

                cmd.Parameters.AddWithValue("@nomina_id", detalle.NominaId);
                cmd.Parameters.AddWithValue("@trabajador_id", detalle.TrabajadorId);
                cmd.Parameters.AddWithValue("@remuneracion_bruta", detalle.RemuneracionBruta);
                cmd.Parameters.AddWithValue("@sueldo_basico", detalle.SueldoBasico);
                cmd.Parameters.AddWithValue("@asignacion_familiar", detalle.AsignacionFamiliar);
                cmd.Parameters.AddWithValue("@horas_extras", detalle.HorasExtras);
                cmd.Parameters.AddWithValue("@bonos_regulares", detalle.BonosRegulares);
                cmd.Parameters.AddWithValue("@otros_ingresos", detalle.OtrosIngresos);
                cmd.Parameters.AddWithValue("@sistema_pension_aplicado", (object)detalle.SistemaPensionAplicado ?? DBNull.Value);
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
                cmd.Parameters.AddWithValue("@total_ingresos", detalle.TotalIngresos);
                cmd.Parameters.AddWithValue("@total_descuentos", detalle.TotalDescuentos);
                cmd.Parameters.AddWithValue("@neto_pagar", detalle.NetoPagar);
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
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CREACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }
        
    }
}