using System;
using System.Collections.Generic;
using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;

namespace capa_persistencia.modulo_principal
{
    public class SistemaPension
    {
        public int TipoPensionId { get; set; }
        public string TipoPensionNombre { get; set; }
        public string TipoPensionEntidad { get; set; }
    }

    public class SistemasPensiones
    {
        private readonly AccesoSQLServer _accesoSQL;

        public SistemasPensiones()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<SistemaPension> ObtenerSistemasPensiones()
        {
            var pensiones = new List<SistemaPension>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_sistema_pensiones");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pension = new SistemaPension
                        {
                            TipoPensionId = reader.GetInt32(reader.GetOrdinal("tipo_pension_id")),
                            TipoPensionNombre = reader.GetString(reader.GetOrdinal("tipo_pension_nombre")),
                            TipoPensionEntidad = reader.GetString(reader.GetOrdinal("tipo_pension_entidad"))
                        };
                        pensiones.Add(pension);
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

            return pensiones;
        }
    }
}