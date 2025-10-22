using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.modulo_principal
{
    public class AsignacionFamiliar
    {
        private readonly AccesoSQLServer _accesoSQL;

        public AsignacionFamiliar()
        {
            _accesoSQL = new AccesoSQLServer();
        }

        public decimal ObtenerAsignacionFamiliarPorTrabajador(int trabajadorId)
        {
            try
            {
                _accesoSQL.AbrirConexion();

                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("proc_obtener_asignacion_familiar_trabajador");
                cmd.Parameters.AddWithValue("@trabajador_id", trabajadorId);

                var resultado = cmd.ExecuteScalar();

                return (resultado == null || resultado == DBNull.Value)
                    ? 0m
                    : Convert.ToDecimal(resultado);
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally
            {
                _accesoSQL.CerrarConexion();
            }
        }
    }
    }
