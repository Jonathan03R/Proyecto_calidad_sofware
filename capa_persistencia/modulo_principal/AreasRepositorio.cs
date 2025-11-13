using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class AreasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public AreasRepositorio(AccesoSQLServer conexion)
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<Area> ObtenerAreas()
        {
            List<Area> areas = new List<Area>();

            try
            {
                System.Diagnostics.Debug.WriteLine("Obteniendo áreas...");

                _accesoSQL.AbrirConexion();

                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areas.Add(new Area
                        {
                            AreaId = Convert.ToInt32(reader["AreaId"]),
                            AreaNombre = reader["NombreArea"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR en ObtenerAreas: " + ex.Message);
                throw; // opcional: relanzas el error a la capa superior
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }

            return areas;
        }

    }
}
