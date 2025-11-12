using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{
    public class TiposHorasExtrasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TiposHorasExtrasRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public List<TipoHoraExtra> ObtenerTiposHorasExtrasActivos()
        {
            var lista = new List<TipoHoraExtra>();
            System.Diagnostics.Debug.WriteLine("Obteniendo tipos de horas extras activas...");

            try
            {
                var cmd = _accesoSQL.ObtenerComandoSQL("SELECT * FROM nomina.tipos_horas_extras WHERE tipos_horas_extras_estado = 'A' ORDER BY tipo_hora_extra_id");

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var tipo = new TipoHoraExtra
                        {
                            TipoHoraExtraId = Convert.ToInt32(dr["tipo_hora_extra_id"]),
                            TiposHorasExtrasCodigo = dr["tipos_horas_extras_codigo"].ToString(),
                            TiposHorasExtrasNombre = dr["tipos_horas_extras_nombre"].ToString(),
                            TiposHorasExtrasMultiplicador = Convert.ToDecimal(dr["tipos_horas_extras_multiplicador"]),
                            TiposHorasExtrasEstado = Convert.ToChar(dr["tipos_horas_extras_estado"]),
                        };
                        lista.Add(tipo);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new Exception("Error al obtener los tipos de horas extras", ex);
            }

            return lista;
        }
    }
}
