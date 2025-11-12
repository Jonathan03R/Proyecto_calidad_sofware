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
            return areas;
        }
    }
}
