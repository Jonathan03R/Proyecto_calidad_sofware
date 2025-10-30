using System;
using System.Collections.Generic;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
// <<<<<<< CapaPresentacionH2
    
// =======
//     // DTO (mover a Dominio si ya lo tienes allí)


// >>>>>>> main
    public class Nominas
    {
        private readonly AccesoSQLServer _accesoSQL;
        public Nominas() { _accesoSQL = new AccesoSQLServer(); }
        
        public int IniciarProcesoPorPeriodo(int periodoId, string observaciones = null)
        {
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_insertar_nomina_cabecera_por_periodo");
                cmd.Parameters.AddWithValue("@periodo_id", periodoId);
                cmd.Parameters.AddWithValue("@nomina_fecha", DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nomina_estado", "Procesando");

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CREACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

        public void ActualizarTotales(int nominaId, int totalEmpleados,
                                      decimal totalBruto, decimal totalDescuentos,
                                      decimal totalNeto, string estadoFinal)
        {
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_actualizar_nomina_totales_por_id");
                cmd.Parameters.AddWithValue("@nomina_id", nominaId);
                cmd.Parameters.AddWithValue("@total_empleados", totalEmpleados);
                cmd.Parameters.AddWithValue("@total_bruto", totalBruto);
                cmd.Parameters.AddWithValue("@total_descuentos", totalDescuentos);
                cmd.Parameters.AddWithValue("@total_neto", totalNeto);
                cmd.Parameters.AddWithValue("@estado_final", estadoFinal);

                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_ACTUALIZACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

        public void ActualizarEstado(int nominaId, string nuevoEstado)
        {
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento(
                    "nomina.proc_actualizar_nomina_estado_por_id");
                cmd.Parameters.AddWithValue("@nomina_id", nominaId);
                cmd.Parameters.AddWithValue("@nuevo_estado", nuevoEstado);

                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_ACTUALIZACION);
            }
            finally { _accesoSQL.CerrarConexion(); }
        }

    }
}
