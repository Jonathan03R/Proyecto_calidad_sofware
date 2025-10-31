using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using capa_dominio;

namespace capa_persistencia.modulo_principal
{

    public class Cargos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Cargos()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<Cargo> ObtenerCargos()
        {
            var listaCargos = new List<Cargo>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_cargos");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cargo = new Cargo
                        {
                            CargoId = reader.GetInt32(reader.GetOrdinal("cargo_id")),
                            CargoNombre = reader.GetString(reader.GetOrdinal("cargo_nombre"))
                        };
                        listaCargos.Add(cargo);
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

            return listaCargos;
        }
    }
}