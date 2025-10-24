
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using capa_dominio;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class AdelantoSueldoRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public AdelantoSueldoRepositorio()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public IList<AdelantoSueldo> ObtenerPorTrabajador(
            int trabajadorId,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var lista = new List<AdelantoSueldo>();

            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_adelantos_por_trabajador");

                cmd.Parameters.Add(new SqlParameter("@trabajador_id", SqlDbType.Int) { Value = trabajadorId });
                cmd.Parameters.Add(new SqlParameter("@fecha_inicio", SqlDbType.Date) { Value = (object)fechaInicio ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@fecha_fin", SqlDbType.Date) { Value = (object)fechaFin ?? DBNull.Value });

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                {
                    var adelanto = new AdelantoSueldo
                    {
                        AdelantoId = reader.GetInt32(reader.GetOrdinal("adelanto_id")),
                        AdelantoMonto = reader.GetDecimal(reader.GetOrdinal("adelanto_monto")),
                        AdelantoFecha = reader.GetDateTime(reader.GetOrdinal("adelanto_fecha")),
                        AdelantoMotivo = reader.GetString(reader.GetOrdinal("adelanto_motivo")),
                        AdelantoObservaciones = reader.IsDBNull(reader.GetOrdinal("adelanto_observaciones"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("adelanto_observaciones")),
                        // Mapea el Trabajador del dominio con su Id
                        Trabajador = new Trabajador
                        {
                            // Ajusta el nombre de la propiedad si tu clase difiere
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id"))
                        }
                    };

                    lista.Add(adelanto);
                }
            }
            catch (SqlException ex)
            {
                // Usa tu excepción de persistencia estándar
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA, ex.Message);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return lista;
        }
    }
}
