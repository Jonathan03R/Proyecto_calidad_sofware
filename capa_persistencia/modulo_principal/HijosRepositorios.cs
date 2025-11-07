using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class HijosRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public HijosRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public List<Hijo> ObtenerHijosPorTrabajador(int trabajadorId)
        {
            var hijos = new List<Hijo>();

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("personal.proc_obtener_hijos_por_trabajador");
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var hijo = new Hijo
                        {
                            HijoId = Convert.ToInt32(dr["hijo_id"]),
                            TrabajadorId = Convert.ToInt32(dr["trabajador_id"]),
                            Nombres = dr["hijo_nombres"].ToString(),
                            Apellidos = dr["hijo_apellidos"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(dr["hijo_fecha_nacimiento"]),
                            Estudia = Convert.ToBoolean(dr["hijo_estudia"]),
                            TieneDiscapacidad = Convert.ToBoolean(dr["hijo_tiene_discapacidad"]),
                            Estado = Convert.ToChar(dr["hijo_estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["hijo_fecha_creacion"])
                        };
                        hijos.Add(hijo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los hijos del trabajador", ex);
            }

            return hijos;
        }
    }
}