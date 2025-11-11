using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class ReporteNomina
    {
        private readonly AccesoSQLServer conexion;

        public ReporteNomina(AccesoSQLServer accesoSQLServer)
        {
            this.conexion = accesoSQLServer;
        }

        public List<ReporteNominaDTO> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            List<ReporteNominaDTO> listaReporte = new List<ReporteNominaDTO>();

            try
            {
                SqlCommand cmd = conexion.ObtenerComandoDeProcedimiento("proc_Consultar_Nomina_Por_Periodo");
                cmd.Parameters.AddWithValue("@PeriodoID", periodoId);
                cmd.Parameters.AddWithValue("@CargoID", (object)cargoId ?? DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ReporteNominaDTO reporte = new ReporteNominaDTO
                        {
                            // Datos del Trabajador
                            CodigoTrabajador = dr["CodigoTrabajador"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            TipoDeIdentificacion = dr["Tipo de Identificacion"].ToString(),
                            NumeroIdentificacion = dr["NumeroIdentificacion"].ToString(),
                            SistemaPension = dr["SistemaPension"].ToString(),
                            TipoTrabajador = dr["TipoTrabajador"].ToString(),
                            FechaInicioContrato = dr["FechaInicioContrato"] != DBNull.Value
                                ? DateTime.Parse(dr["FechaInicioContrato"].ToString()).ToString("yyyy-MM-dd")
                                : null,
                            FechaFinContrato = dr["FechaFinContrato"] != DBNull.Value
                                ? DateTime.Parse(dr["FechaFinContrato"].ToString()).ToString("yyyy-MM-dd")
                                : null,

                            // Jornada Laboral
                            TipoDeJornadaPactada = dr["TipoDeJornadaPactada"].ToString(),
                            HorasSemanalesPactadas = dr["HorasSemanalesPactadas"] != DBNull.Value
                                ? (decimal?)Convert.ToDecimal(dr["HorasSemanalesPactadas"])
                                : null,
                            HorasTrabajadasEstimadas = dr["HorasTrabajadasEstimadas"] != DBNull.Value
                                ? (decimal?)Convert.ToDecimal(dr["HorasTrabajadasEstimadas"])
                                : null,
                            HorasExtrasReales = dr["HorasExtrasReales"] != DBNull.Value
                                ? (decimal?)Convert.ToDecimal(dr["HorasExtrasReales"])
                                : null,

                            // Ingresos
                            SueldoBasico = Convert.ToDecimal(dr["SueldoBasico"]),
                            AsignacionFamiliar = Convert.ToDecimal(dr["AsignacionFamiliar"]),
                            MontoHorasExtras = Convert.ToDecimal(dr["MontoHorasExtras"]),
                            MontoBonos = Convert.ToDecimal(dr["MontoBonos"]),
                            OtrosIngresos = Convert.ToDecimal(dr["OtrosIngresos"]),
                            TotalHaberesBruto = Convert.ToDecimal(dr["TotalHaberesBruto"]),
                            TotalHaberes = Convert.ToDecimal(dr["TotalHaberes"]),

                            // Descuentos Legales
                            AporteSistemaPension = Convert.ToDecimal(dr["AporteSistemaPension"]),
                            DescuentoONP = Convert.ToDecimal(dr["DescuentoONP"]),
                            DescuentoAFP = Convert.ToDecimal(dr["DescuentoAFP"]),
                            RetencionImpuestoRenta = Convert.ToDecimal(dr["RetencionImpuestoRenta"]),

                            // Aportes del Empleador
                            AporteEsSalud = Convert.ToDecimal(dr["AporteEsSalud"]),
                            BaseImponibleEsSalud = Convert.ToDecimal(dr["BaseImponibleEsSalud"]),

                            // Otros Descuentos
                            DescuentoFaltas = Convert.ToDecimal(dr["DescuentoFaltas"]),
                            DescuentoAdelantos = Convert.ToDecimal(dr["DescuentoAdelantos"]),
                            OtrosDescuentos = Convert.ToDecimal(dr["OtrosDescuentos"]),

                            // Totales
                            TotalDescuentos = Convert.ToDecimal(dr["TotalDescuentos"]),
                            NetoPagar = Convert.ToDecimal(dr["NetoPagar"]),

                            // Datos de Contexto
                            PeriodoNomina = dr["PeriodoNomina"].ToString()
                        };

                        listaReporte.Add(reporte);
                    }
                }
            }
            catch (FormatException ex)
            {
                throw new Exception($"Error de formato al convertir datos: {ex.Message}. Verifique los tipos de datos en la consulta.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al ejecutar proc_Consultar_Nomina_Por_Periodo: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar nómina por período.", ex);
            }
            

            return listaReporte;
        }

        /// <summary>
        /// Lista los períodos disponibles de nómina.
        /// </summary>
        public List<Periodo> ListarPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            List<Periodo> lista = new List<Periodo>();

            try
            {
                SqlCommand cmd = conexion.ObtenerComandoDeProcedimiento("proc_Listar_Periodos");
                cmd.Parameters.AddWithValue("@PeriodoID", periodoId.HasValue ? (object)periodoId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@PeriodoNombre", !string.IsNullOrEmpty(periodoNombre) ? (object)periodoNombre : DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Periodo periodo = new Periodo
                        {
                            PeriodoId = Convert.ToInt32(dr["periodo_id"]),
                            PeriodoNombre = dr["periodo_nombre"].ToString(),
                            PeriodoFechaInicio = Convert.ToDateTime(dr["periodo_fecha_inicio"]),
                            PeriodoFechaFin = Convert.ToDateTime(dr["periodo_fecha_fin"]),
                            PeriodoEstado = dr["periodo_estado"].ToString()
                        };
                        lista.Add(periodo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar períodos de nómina.", ex);
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return lista;
        }
    }
}
