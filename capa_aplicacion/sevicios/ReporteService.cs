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
        private readonly AccesoSQLServer conexion;

        public ReporteService()
        {
            conexion = new AccesoSQLServer();
            reporteNomina = new ReporteNomina(conexion);
        }

        public List<ReporteNominaDTO> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            List<ReporteNominaDTO> listaReporte;
            try
            {
                conexion.AbrirConexion();
                listaReporte = reporteNomina.ConsultarNominaPorPeriodo(periodoId, cargoId);
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listaReporte;
        }

        public List<Periodo> ListarPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            List<Periodo> listaPeriodo;
            try
            {
                conexion.AbrirConexion();
                listaPeriodo = reporteNomina.ListarPeriodos(periodoId, periodoNombre);
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listaPeriodo;
        }
    }
}
