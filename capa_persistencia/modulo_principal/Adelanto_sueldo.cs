using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{

    /// Esto tiene que estar en la capa de dominio
    public class AdelantoSueldo
    {
        private int adelantoId;
        private int trabajadorId;
        private decimal adelantoMonto;
        private DateTime adelantoFecha;
        private string adelantoMotivo;
        private string adelantoObservaciones;

        public int AdelantoId { get => adelantoId; set => adelantoId = value; }
        public int TrabajadorId { get => trabajadorId; set => trabajadorId = value; }
        public decimal AdelantoMonto { get => adelantoMonto; set => adelantoMonto = value; }
        public DateTime AdelantoFecha { get => adelantoFecha; set => adelantoFecha = value; }
        public string AdelantoMotivo { get => adelantoMotivo; set => adelantoMotivo = value; }
        public string AdelantoObservaciones { get => adelantoObservaciones; set => adelantoObservaciones = value; }

        public bool EsMontoValido()
        {
            return adelantoMonto > 0;
        }

        public bool EsReciente()
        {
            return (DateTime.Now - adelantoFecha).TotalDays <= 30;
        }

    }

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

                comando.Parameters.AddWithValue("@trabajador_id", adelanto.TrabajadorId);
                comando.Parameters.AddWithValue("@adelanto_monto", adelanto.AdelantoMonto);
                comando.Parameters.AddWithValue("@adelanto_fecha", adelanto.AdelantoFecha);
                comando.Parameters.AddWithValue("@adelanto_motivo", adelanto.AdelantoMotivo);
                comando.Parameters.AddWithValue("@adelanto_observaciones", adelanto.AdelantoObservaciones ?? (object)DBNull.Value);

                comando.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CREACION);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }
    }
}
