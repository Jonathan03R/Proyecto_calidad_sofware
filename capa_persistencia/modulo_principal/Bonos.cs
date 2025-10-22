using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class Bonos

    {
        public class Bono
        {
            public int BonoId { get; set; }
            public int TrabajadorId { get; set; }
            public string BonoTipo { get; set; }
            public string BonoConcepto { get; set; }
            public decimal BonoMonto { get; set; }
            public DateTime BonoFecha { get; set; }
            public string BonoObservaciones { get; set; }
        }

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
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
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
