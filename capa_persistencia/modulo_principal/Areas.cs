//using System;
//using System.Collections.Generic;
//using capa_persistencia.modulo_base;
//using Microsoft.Data.SqlClient;
//using capa_dominio;

//namespace capa_persistencia.modulo_principal
//{

//    public class Areas
//    {
//        private readonly AccesoSQLServer _accesoSQL;

//        public Areas()
//        {
//            _accesoSQL = new AccesoSQLServer();
//        }

//        public List<Area> ObtenerAreasTrabajo()
//        //{
//            var areas = new List<Area>();

//            try
//            {
//                _accesoSQL.AbrirConexion();
//                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_areas_trabajo");

//                using (var reader = comando.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        var area = new Area
//                        {
//                            AreaId = reader.GetInt32(reader.GetOrdinal("area_id")),
//                            AreaNombre = reader.GetString(reader.GetOrdinal("area_nombre")),
//                            AreaDescripcion = reader.GetString(reader.GetOrdinal("area_descripcion")),
//                            SedeNombre = reader.GetString(reader.GetOrdinal("sede_nombre")),
//                            SedeDireccion = reader.GetString(reader.GetOrdinal("sede_direccion")),
//                            SedeDepartamento = reader.GetString(reader.GetOrdinal("sede_departamento")),
//                            SedeProvincia = reader.GetString(reader.GetOrdinal("sede_provincia"))
//                        };
//                        areas.Add(area);
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
//            }
//            finally
//            {
//                _accesoSQL.CerrarConexion();
//            }

//            return areas;
//        }
//    }
//}