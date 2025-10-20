using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class Cargo
    {
        public int CargoId { get; set; }
        public string CargoNombre { get; set; }
    }

    public class Cargos
    {
        private readonly AccesoSQLServer _accesoSQL;

        public Cargos()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<Cargo> ObtenerCargos()
        {
            var cargos = new List<Cargo>();

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
                        cargos.Add(cargo);
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

            return cargos;
        }
    }
}