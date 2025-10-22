using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;


namespace capa_persistencia.modulo_principal
{
    public class Adelanto_sueldo
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Adelanto_sueldo()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public void InsertarAdelantoSueldo(AdelantoSueldo adelanto)
        {
            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_insertar_adelanto_sueldo");

                comando.Parameters.AddWithValue("@trabajador_id", adelanto.Trabajador.TrabajadorId);
                comando.Parameters.AddWithValue("@adelanto_monto", adelanto.AdelantoMonto);
                comando.Parameters.AddWithValue("@adelanto_fecha", adelanto.AdelantoFecha);
                comando.Parameters.AddWithValue("@adelanto_motivo", adelanto.AdelantoMotivo);
                comando.Parameters.AddWithValue("@adelanto_observaciones", adelanto.AdelantoObservaciones ?? (object)DBNull.Value);

                comando.ExecuteNonQuery();
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }
       
    }
}


