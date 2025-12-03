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
        private readonly ReporteNominaRepositorio reporteNomina;
        private readonly PeriodosRepositorio periodosRepositorio;
        private readonly AccesoSQLServer conexion;

        public ReporteService()
        {
            conexion = new AccesoSQLServer();
            reporteNomina = new ReporteNominaRepositorio(conexion);
            periodosRepositorio = new PeriodosRepositorio(conexion);    
        }

        public List<ReporteNominaDTO> ConsultarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            try
            {
                conexion.AbrirConexion();
                return reporteNomina.ConsultarNominaPorPeriodo(periodoId, cargoId);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
    }
}
