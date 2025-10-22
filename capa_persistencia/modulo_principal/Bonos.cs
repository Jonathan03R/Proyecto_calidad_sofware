using capa_dominio;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class Bonos_Repositorio

    {

        private readonly AccesoSQLServer _accesoSQL = new AccesoSQLServer();
        
        public List<Bono> ObtenerBonosPorTrabajador(int trabajadorId)
        {
            var lista = new List<Bono>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_bonos_por_trabajador");
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Bono
                        {
                            BonoId = reader.GetInt32(reader.GetOrdinal("bono_id")),
                            Trabajador = new Trabajador
                            {
                                TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id"))
                            },
                            BonoTipo = reader.IsDBNull(reader.GetOrdinal("bono_tipo")) ? null : reader.GetString(reader.GetOrdinal("bono_tipo")),
                            BonoConcepto = reader.IsDBNull(reader.GetOrdinal("bono_concepto")) ? null : reader.GetString(reader.GetOrdinal("bono_concepto")),
                            BonoMonto = reader.GetDecimal(reader.GetOrdinal("bono_monto")),
                            BonoFecha = reader.GetDateTime(reader.GetOrdinal("bono_fecha")),
                            BonoObservaciones = reader.IsDBNull(reader.GetOrdinal("bono_observaciones")) ? null : reader.GetString(reader.GetOrdinal("bono_observaciones"))
                        });
                    }
                }
            }
            catch { throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA); }
            finally { _accesoSQL.CerrarConexion(); }
            return lista;
        }
    }
}
