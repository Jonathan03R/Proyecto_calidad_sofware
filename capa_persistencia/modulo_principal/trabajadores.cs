using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_persistencia.modulo_base;

namespace capa_persistencia.modulo_principal
{
    //esto es una entidad @yanmir lo tiene que poner corectamente en su capa de entidades
    public class Empleado
    {
        public int TrabajadorId { get; set; }
        public string CodigoTrabajador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Prefijo { get; set; }
        public string Identificacion { get; set; }
    }

    public class trabajadores
    {
        private readonly AccesoSQLServer _accesoSQL;

        public trabajadores()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public List<Empleado> ObtenerEmpleados(int? pagina = null, int? cantidad = null)
        {
            var empleados = new List<Empleado>();

            try
            {
                _accesoSQL.AbrirConexion();
                var comando = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_empleados");
                comando.Parameters.AddWithValue("@pagina", pagina ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@cantidad", cantidad ?? (object)DBNull.Value);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var empleado = new Empleado
                        {
                            TrabajadorId = reader.GetInt32(reader.GetOrdinal("trabajador_id")),
                            CodigoTrabajador = reader.GetString(reader.GetOrdinal("codigo_trabajador")),
                            Nombres = reader.GetString(reader.GetOrdinal("nombres")),
                            Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                            TipoIdentificacion = reader.GetString(reader.GetOrdinal("tipo_identificacion")),
                            Prefijo = reader.GetString(reader.GetOrdinal("prefijo")),
                            Identificacion = reader.GetString(reader.GetOrdinal("identificacion"))
                        };
                        empleados.Add(empleado);
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

            return empleados;
        }
    }
}
