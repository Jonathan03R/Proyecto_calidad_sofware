using capa_dominio;
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

        public List<DetalleNomina> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            try
            {
                if (periodoId <= 0)
                {
                    var periodos = reporteNomina.ListarPeriodos();
                    if (periodos == null || periodos.Count == 0)
                        return new List<DetalleNomina>();

                    periodoId = periodos.First().PeriodoId;
                }

                var detalles = reporteNomina.ConsultarNominaPorPeriodo(periodoId, cargoId);
                return detalles ?? new List<DetalleNomina>();
            }
            catch (Exception ex)
            {
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
