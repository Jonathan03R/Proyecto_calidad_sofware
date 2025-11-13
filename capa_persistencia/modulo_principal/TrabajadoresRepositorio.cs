using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    public class TrabajadoresRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public TrabajadoresRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public List<Trabajador> ObtenerEmpleados(int? pagina = null, int? cantidad = null)
        {
            var empleados = new List<Trabajador>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_empleados");
                comando.Parameters.AddWithValue("@pagina", pagina ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@cantidad", cantidad ?? (object)DBNull.Value);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var trabajador = new Trabajador
                        {
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            Codigo = reader.GetString(reader.GetOrdinal("codigo_trabajador")),
                            Nombres = reader.GetString(reader.GetOrdinal("nombres")),
                            Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                            TipoIdentificacion = reader.GetString(reader.GetOrdinal("tipo_identificacion")),
                            Identificacion = reader.GetString(reader.GetOrdinal("identificacion")),
                            Estado = 'A', // por defecto activo
                        };

                        empleados.Add(trabajador);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }

            return empleados;
        }

        public List<PersonaSinContratoDTO> ObtenerPersonasSinContratoActivo()
        {
            var personas = new List<PersonaSinContratoDTO>();

            try
            {
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("Personal.proc_obtener_personas_sin_contrato_activo");

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var persona = new PersonaSinContratoDTO
                        {
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            PersonaApellido = reader.GetString(reader.GetOrdinal("persona_apellido")),
                            PersonaNombre = reader.GetString(reader.GetOrdinal("persona_nombre")),
                            PersonaIdentificacion = reader.GetString(reader.GetOrdinal("persona_identificacion")),
                            EstadoContrato = reader.GetString(reader.GetOrdinal("EstadoContrato"))
                        };

                        personas.Add(persona);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo personas sin contrato activo: {ex.Message}");
                throw new ExcepcionTrabajador(ExcepcionTrabajador.ERROR_DE_CONSULTA);
            }

            return personas;
        }
    }
}