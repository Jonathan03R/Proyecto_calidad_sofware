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
    public class ReporteNominaRepositorio
    {
        private readonly AccesoSQLServer conexion;

        public ReporteNominaRepositorio(AccesoSQLServer accesoSQLServer)
        {
            this.conexion = accesoSQLServer;
        }

        public List<ReporteNominaDTO> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            var lista = new List<ReporteNominaDTO>();

            try
            {
                SqlCommand cmd = conexion.ObtenerComandoDeProcedimiento("proc_Consultar_Nomina_Por_Periodo");
                cmd.Parameters.AddWithValue("@PeriodoID", periodoId);
                cmd.Parameters.AddWithValue("@CargoID", (object)cargoId ?? DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var dto = new ReporteNominaDTO
                        {
                            CodigoTrabajador = dr["CodigoTrabajador"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            TipoDeIdentificacion = dr["TipoIdentificacion"].ToString(),
                            NumeroIdentificacion = dr["NumeroIdentificacion"].ToString(),
                            SistemaPension = dr["SistemaPension"].ToString(),
                            TipoTrabajador = dr["TipoTrabajador"].ToString(),
                            FechaInicioContrato = dr["FechaInicioContrato"] == DBNull.Value
                                                  ? null
                                                  : Convert.ToDateTime(dr["FechaInicioContrato"]).ToString("yyyy-MM-dd"),

                            FechaFinContrato = dr["FechaFinContrato"] == DBNull.Value
                                                  ? null
                                                  : Convert.ToDateTime(dr["FechaFinContrato"]).ToString("yyyy-MM-dd"),
                            HorasSemanalesPactadas = dr["HorasSemanalesPactadas"] == DBNull.Value
                                                     ? null
                                                     : (decimal?)Convert.ToDecimal(dr["HorasSemanalesPactadas"]),
                            HorasExtrasReales = dr["HorasExtrasReales"] == DBNull.Value
                                                ? null
                                                : (decimal?)Convert.ToDecimal(dr["HorasExtrasReales"]),
                            SueldoBasico = Convert.ToDecimal(dr["SueldoBasico"]),
                            AsignacionFamiliar = Convert.ToDecimal(dr["AsignacionFamiliar"]),
                            MontoHorasExtras = Convert.ToDecimal(dr["MontoHorasExtras"]),
                            MontoBonos = Convert.ToDecimal(dr["MontoBonos"]),
                            OtrosIngresos = Convert.ToDecimal(dr["OtrosIngresos"]),
                            TotalHaberesBruto = Convert.ToDecimal(dr["TotalHaberesBruto"]),
                            TotalHaberes = Convert.ToDecimal(dr["TotalHaberes"]),
                            AporteSistemaPension = Convert.ToDecimal(dr["AporteSistemaPension"]),
                            RetencionImpuestoRenta = Convert.ToDecimal(dr["RetencionImpuestoRenta"]),
                            AporteEsSalud = Convert.ToDecimal(dr["AporteEsSalud"]),
                            BaseImponibleEsSalud = Convert.ToDecimal(dr["BaseImponibleEsSalud"]),
                            DescuentoTardanzas = Convert.ToDecimal(dr["DescuentoTardanzas"]),
                            DescuentoFaltas = Convert.ToDecimal(dr["DescuentoFaltas"]),
                            // ===== TOTALES =====
                            TotalDescuentos = Convert.ToDecimal(dr["TotalDescuentos"]),
                            NetoPagar = Convert.ToDecimal(dr["NetoPagar"]),
                        };

                        lista.Add(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error al consultar nómina por período", ex);
            }

            return lista;
        }
    }
}
