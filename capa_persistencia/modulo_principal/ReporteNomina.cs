using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_dominio;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class ReporteNomina
    {
        private readonly AccesoSQLServer conexion;

        public ReporteNomina()
        {
            conexion = new AccesoSQLServer();
            conexion.AbrirConexion();
        }

        // Se agrego esto porque Copilot dijo que esto unia el nombre completo
        // en nombres y apellidos de forma segura
        private static string SafeGet(SqlDataReader dr, string columnName)
        {
            try
            {
                int idx = dr.GetOrdinal(columnName);
                return dr.IsDBNull(idx) ? null : dr.GetString(idx);
            }
            catch
            {
                return null;
            }
        }

        private static void SplitFullName(string fullName, out string nombres, out string apellidos)
        {
            nombres = string.Empty;
            apellidos = string.Empty;

            if (string.IsNullOrWhiteSpace(fullName)) return;

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                nombres = parts[0];
                apellidos = string.Empty;
            }
            else
            {
                apellidos = parts[parts.Length - 1];
                nombres = string.Join(" ", parts.Take(parts.Length - 1));
            }
        }

        public List<DetalleNomina> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            List<DetalleNomina> lista = new List<DetalleNomina>();

            try
            {

                SqlCommand cmd = conexion.ObtenerComandoDeProcedimiento("proc_Consultar_Nomina_Por_Periodo");
                cmd.Parameters.AddWithValue("@PeriodoID", periodoId);
                cmd.Parameters.AddWithValue("@CargoID", cargoId.HasValue ? (object)cargoId.Value : DBNull.Value);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Intentamos obtener nombres y apellidos por separado; si no existen, separamos NombreCompleto.
                        string nombres = SafeGet(dr, "Nombres") ?? SafeGet(dr, "nombres");
                        string apellidos = SafeGet(dr, "Apellidos") ?? SafeGet(dr, "apellidos");

                        if (string.IsNullOrWhiteSpace(nombres) && string.IsNullOrWhiteSpace(apellidos))
                        {
                            var full = SafeGet(dr, "NombreCompleto") ?? SafeGet(dr, "nombre_completo") ?? SafeGet(dr, "Nombre") ?? SafeGet(dr, "nombre");
                            SplitFullName(full, out nombres, out apellidos);
                        }

                        Trabajador t = new Trabajador
                        {
                            Codigo = SafeGet(dr, "CodigoTrabajador") ?? SafeGet(dr, "codigo_trabajador") ?? string.Empty,
                            Nombres = nombres ?? string.Empty,
                            Apellidos = apellidos ?? string.Empty,
                            TipoIdentificacion = SafeGet(dr, "Tipo de Identificacion") ?? SafeGet(dr, "tipo_identificacion") ?? string.Empty,
                            Identificacion = SafeGet(dr, "NumeroIdentificacion") ?? SafeGet(dr, "numero_identificacion") ?? SafeGet(dr, "identificacion") ?? string.Empty
                        };

                        Cargo cargo = new Cargo
                        {
                            CargoNombre = dr["Tipo de Cargo"].ToString()
                        };

                        DateTime? fechaFin = dr["FechaFinContrato"] is DBNull ? (DateTime?)null : Convert.ToDateTime(dr["FechaFinContrato"]);

                        Contrato c = new Contrato
                        {
                            ContratoFechaInicio = Convert.ToDateTime(dr["FechaInicioContrato"]),
                            ContratoFechaFin = fechaFin,
                            // Asignamos el trabajador al contrato para que la cadena completa esté disponible donde se use.
                            Trabajador = t
                        };

                        DetalleNomina detalle = new DetalleNomina
                        {
                            // Asignación de objetos referenciados
                            Contrato = c,

                            // Mapeo de Ingresos
                            SueldoBasico = Convert.ToDecimal(dr["SueldoBasico"]),
                            AsignacionFamiliar = Convert.ToDecimal(dr["AsignacionFamiliar"]),
                            HorasExtras = Convert.ToDecimal(dr["MontoHorasExtras"]),
                            BonosRegulares = Convert.ToDecimal(dr["MontoBonos"]),
                            OtrosIngresos = Convert.ToDecimal(dr["OtrosIngresos"]),

                            // Mapeo de Descuentos/Aportes (se debe usar manejo de DBNull si aplican)
                            AporteEssalud = Convert.ToDecimal(dr["AporteEsSalud"]),
                            AporteONP = dr["DescuentoONP"] is DBNull ? 0m : Convert.ToDecimal(dr["DescuentoONP"]),
                            DescuentoAFP = dr["DescuentoAFP"] is DBNull ? 0m : Convert.ToDecimal(dr["DescuentoAFP"]),
                            ImpuestoRentaMensual = Convert.ToDecimal(dr["RetencionImpuestoRenta"]),
                            DescuentoFaltas = Convert.ToDecimal(dr["DescuentoFaltas"]),
                            DescuentoAdelantos = Convert.ToDecimal(dr["DescuentoAdelantos"]),

                            // Mapeo de Totales
                            TotalIngresos = Convert.ToDecimal(dr["TotalHaberes"]),
                            TotalDescuentos = Convert.ToDecimal(dr["TotalDescuentos"]),
                            NetoPagar = Convert.ToDecimal(dr["NetoPagar"])

                        };

                        Periodo periodo = new Periodo
                        {
                            PeriodoNombre = dr["PeriodoNombre"].ToString(),
                        };

                        lista.Add(detalle);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar nómina por período.", ex);
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return lista;
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
