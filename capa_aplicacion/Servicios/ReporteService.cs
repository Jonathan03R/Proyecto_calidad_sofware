using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.Servicios
{
    public class ReporteService
    {
        private readonly ReporteNomina reporteNomina;

        public ReporteService()
        {
            reporteNomina = new ReporteNomina();
        }

        public List<ReporteNominaDTO> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            try
            {
                if (periodoId <= 0)
                {
                    var periodos = reporteNomina.ListarPeriodos();
                    if (periodos == null || periodos.Count == 0)
                        return new List<ReporteNominaDTO>();

                    periodoId = periodos.First().PeriodoId;
                }

                var reporte = reporteNomina.ConsultarNominaPorPeriodo(periodoId, cargoId);
                return reporte ?? new List<ReporteNominaDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Error exacto: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("🔍 Traza del error: " + ex.StackTrace);

                throw new Exception("Error al ConsultarNominaPorPeriodo", ex);
            }
        }

        // Puse Listar Pwriodos por aqui ya que no se si lo van a poner en otro servicio
        // Por el momento lo dejo aqui
        public List<Periodo> ListarPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            try
            {
                var periodos = reporteNomina.ListarPeriodos(periodoId, periodoNombre);
                return periodos ?? new List<Periodo>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error en ReporteService.ListarPeriodos", ex);
            }
        }
    }
}
