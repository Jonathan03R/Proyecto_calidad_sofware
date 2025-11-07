using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.servicio
{
    public class reporteService
    {

        private SqlConnection conexio;
        private ReporteNomina reporteNomina;

        public reporteService()
        {
            reporteNomina = new ReporteNomina();
        }
    }
}
