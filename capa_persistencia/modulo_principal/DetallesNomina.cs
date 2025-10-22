using System;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System.Data;

namespace capa_persistencia.modulo_principal
{
    public class DetallesNomina
    {
        private readonly AccesoSQLServer _accesoSQL;

        public DetallesNomina() { _accesoSQL = new AccesoSQLServer(); }

        // I-03: INSERT detalle -> devuelve @detalle_nomina_id
        public int InsertarDetalleNomina(
            int nominaId,
            int trabajadorId,
            decimal remuneracionBruta,
            decimal sueldoBasico,
            decimal asignacionFamiliar,
            decimal horasExtras,
            decimal bonosRegulares,
            decimal otrosIngresos,
            string sistemaPensionAplicado,
            decimal aporteEssalud,
            decimal aporteOnp,
            decimal descuentoAfp,
            decimal remuneracionAcumuladaAnual,
            decimal baseImponibleAnual,
            decimal impuestoRentaAnual,
            decimal impuestoRentaMensual,
            decimal uitValor,
            decimal deduccion7Uit,
            decimal descuentoTardanzas,
            decimal descuentoFaltas,
            decimal descuentoAdelantos,
            decimal otrosDescuentos,
            decimal totalIngresos,
            decimal totalDescuentos,
            decimal netoPagar,
            bool tieneErrores = false,
            string mensajeError = null
        )
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_detalle_nomina_por_trabajador");

                cmd.Parameters.AddWithValue("@nomina_id", nominaId);
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);
                cmd.Parameters.AddWithValue("@remuneracion_bruta", remuneracionBruta);
                cmd.Parameters.AddWithValue("@sueldo_basico", sueldoBasico);
                cmd.Parameters.AddWithValue("@asignacion_familiar", asignacionFamiliar);
                cmd.Parameters.AddWithValue("@horas_extras", horasExtras);
                cmd.Parameters.AddWithValue("@bonos_regulares", bonosRegulares);
                cmd.Parameters.AddWithValue("@otros_ingresos", otrosIngresos);
                cmd.Parameters.AddWithValue("@sistema_pension_aplicado", (object)sistemaPensionAplicado ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@aporte_essalud", aporteEssalud);
                cmd.Parameters.AddWithValue("@aporte_onp", aporteOnp);
                cmd.Parameters.AddWithValue("@descuento_afp", descuentoAfp);
                cmd.Parameters.AddWithValue("@remuneracion_acumulada_anual", remuneracionAcumuladaAnual);
                cmd.Parameters.AddWithValue("@base_imponible_anual", baseImponibleAnual);
                cmd.Parameters.AddWithValue("@impuesto_renta_anual", impuestoRentaAnual);
                cmd.Parameters.AddWithValue("@impuesto_renta_mensual", impuestoRentaMensual);
                cmd.Parameters.AddWithValue("@uit_valor", uitValor);
                cmd.Parameters.AddWithValue("@deduccion_7uit", deduccion7Uit);
                cmd.Parameters.AddWithValue("@descuento_tardanzas", descuentoTardanzas);
                cmd.Parameters.AddWithValue("@descuento_faltas", descuentoFaltas);
                cmd.Parameters.AddWithValue("@descuento_adelantos", descuentoAdelantos);
                cmd.Parameters.AddWithValue("@otros_descuentos", otrosDescuentos);
                cmd.Parameters.AddWithValue("@total_ingresos", totalIngresos);
                cmd.Parameters.AddWithValue("@total_descuentos", totalDescuentos);
                cmd.Parameters.AddWithValue("@neto_pagar", netoPagar);
                cmd.Parameters.AddWithValue("@tiene_errores", tieneErrores);
                cmd.Parameters.AddWithValue("@mensaje_error", (object)mensajeError ?? DBNull.Value);

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
