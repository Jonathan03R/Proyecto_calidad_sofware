using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using capa_dominio;

namespace capa_persistencia.modulo_principal
{

    public class Cargos
    {
        private readonly AccesoSQLServer conexion;

        public Cargos(AccesoSQLServer accesoSQLServer)
        {
            this.conexion = accesoSQLServer;
        }

        public List<Cargo> ObtenerCargos()
        {
            var listaCargos = new List<Cargo>();

            try
            {
                conexion.AbrirConexion();
                var comando = conexion.ObtenerComandoDeProcedimiento("proc_obtener_cargos");

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
                conexion.CerrarConexion();
            }

            return listaCargos;
        }
    }
}