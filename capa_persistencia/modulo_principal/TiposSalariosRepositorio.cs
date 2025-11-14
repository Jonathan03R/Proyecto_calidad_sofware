using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class TiposSalariosRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TiposSalariosRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL;
        }

        public List<TipoSalario> ObtenerTiposSalarios()
        {
            var tipos = new List<TipoSalario>();

            try
            {
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

            return tipos;
        }
    }
}