using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace capa_persistencia.modulo_principal
{
    public class Adelanto_sueldo
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Adelanto_sueldo()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        /// <summary>
        /// Inserta un adelanto de sueldo para un trabajador
        /// </summary>
        /// <param name="adelanto"></param>
        /// <exception cref="ExcepcionTrabajador"></exception>
        public void InsertarAdelantoSueldo(AdelantoSueldoDTO adelanto)
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
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }


        public void ObtenerAdelatosSueldo()
        {
            try
            {
                _accesoSQL.AbrirConexion();

            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }

        /// <summary>
        /// adelantos de sueldo de un trabajador si es null , no tiene adelanto de sueldo
        /// </summary>
        /// <param name="trabajadorId"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        /// <exception cref="ExcepcionTrabajador"></exception>
        public List<AdelantoSueldo> ObtenerAdelantosPorTrabajador(int trabajadorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var adelantos = new List<AdelantoSueldo>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_adelantos_por_trabajador");
                comando.Parameters.AddWithValue("@trabajador_id", trabajadorId);
                comando.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                comando.Parameters.AddWithValue("@fecha_fin", fechaFin);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var adelanto = new AdelantoSueldo
                        {
                            AdelantoId = reader.GetInt32(reader.GetOrdinal("adelanto_id")),
                            AdelantoMonto = reader.GetDecimal(reader.GetOrdinal("adelanto_monto")),
                            AdelantoFecha = reader.GetDateTime(reader.GetOrdinal("adelanto_fecha")),
                            AdelantoMotivo = reader.GetString(reader.GetOrdinal("adelanto_motivo")),
                            AdelantoObservaciones = reader.IsDBNull(reader.GetOrdinal("adelanto_observaciones")) ? null : reader.GetString(reader.GetOrdinal("adelanto_observaciones"))
                        };
                        adelantos.Add(adelanto);
                    }
                }
            }
            catch (Exception)
            {
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return adelantos;
        }
       
    }
}