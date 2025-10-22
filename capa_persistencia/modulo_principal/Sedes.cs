using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using capa_dominio;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class Sedes
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Sedes()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        /// <summary>
        /// obtnego 2 listas, por cada sede obtengo sus areas de las mismas
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ExcepcionTrabajador"></exception>
        public List<Sede> ObtenerSedesConAreas()
        {
            var sedes = new List<Sede>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

                using (var reader = comando.ExecuteReader())
                {
                    var sedeDict = new Dictionary<int, Sede>();

                    while (reader.Read())
                    {
                        int sedeId = reader.GetInt32(reader.GetOrdinal("sede_id"));
                        if (!sedeDict.ContainsKey(sedeId))
                        {
                            var sede = new Sede
                            {
                                SedeId = sedeId,
                                SedeNombre = reader.GetString(reader.GetOrdinal("sede_nombre")),
                                SedeDireccion = reader.GetString(reader.GetOrdinal("sede_direccion")),
                                SedeDepartamento = reader.GetString(reader.GetOrdinal("sede_departamento")),
                                SedeProvincia = reader.GetString(reader.GetOrdinal("sede_provincia")),
                                SedeEstado = 'A',
                                Areas = new List<Area>()
                            };
                            sedeDict[sedeId] = sede;
                            sedes.Add(sede);
                        }

                        var area = new Area
                        {
                            AreaId = reader.GetInt32(reader.GetOrdinal("area_id")),
                            AreaNombre = reader.GetString(reader.GetOrdinal("area_nombre")),
                            AreaDescripcion = reader.GetString(reader.GetOrdinal("area_descripcion"))
                        };

                        sedeDict[sedeId].Areas.Add(area);
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

            return sedes;
        }
    }
}