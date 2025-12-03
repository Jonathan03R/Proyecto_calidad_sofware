using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{

    public class SistemasPensionesRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public SistemasPensionesRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public List<TipoPension> ObtenerSistemasPensiones()
        {
            var pensiones = new List<TipoPension>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_sistema_pensiones");

                using (var reader = comando.ExecuteReader())
                {
                    var ordId = reader.GetOrdinal("tipo_pension_id");
                    var ordNombre = reader.GetOrdinal("tipo_pension_nombre");
                    var ordEntidad = reader.GetOrdinal("tipo_pension_entidad");
                    var ordComision = reader.GetOrdinal("comision_sobre_flujo");

                    while (reader.Read())
                    {
                        pensiones.Add(new TipoPension
                        {
                            TipoPensionId = reader.GetInt32(ordId),
                            Nombre = reader.IsDBNull(ordNombre) ? null : reader.GetString(ordNombre),
                            Entidad = reader.IsDBNull(ordEntidad) ? null : reader.GetString(ordEntidad),
                            ComisionSobreFlujo = reader.IsDBNull(ordComision)? (double?)null: reader.GetDouble(ordComision)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new TrabajadorException(TrabajadorException.ERROR_DE_CONSULTA);
            }

            return pensiones;
        }
    }
}
