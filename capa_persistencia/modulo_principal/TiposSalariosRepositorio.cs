using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class TiposSalariosRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TiposSalariosRepositorio()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<TipoSalario> ObtenerTiposSalarios()
        {
            var tipos = new List<TipoSalario>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_tipos_salarios");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tipo = new TipoSalario
                        {
                            TipoSalarioId = reader.GetInt32(reader.GetOrdinal("tipo_salario_id")),
                            TipoSalarioNombre = reader.GetString(reader.GetOrdinal("tipo_salario_nombre"))
                        };
                        tipos.Add(tipo);
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

            return tipos;
        }
    }
}