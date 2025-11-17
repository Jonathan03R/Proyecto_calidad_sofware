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
            //_accesoSQL = new AccesoSQLServer();
            _accesoSQL = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<Area> ObtenerAreas()
        {
            List<Area> areas = new List<Area>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areas.Add(new Area
                        {
                            AreaId = Convert.ToInt32(reader["area_id"]),
                            AreaNombre = reader["area_nombre"].ToString(),
                            AreaDescripcion = reader["area_descripcion"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las áreas: {ex.Message}");
                throw new Exception("Ocurrió un error al obtener las áreas de trabajo.", ex);
            }

            return areas;
        }

    }
}
