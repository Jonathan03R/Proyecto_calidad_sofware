using capa_dominio;
using capa_persistencia.modulo_base;
using System;
using System.Collections.Generic;

namespace capa_persistencia.modulo_principal
{
    public class HorasTrabajadasRepositorio
    {
        private readonly AccesoSQLServer _accesoSQL;

        public HorasTrabajadasRepositorio(AccesoSQLServer accesoSQL)
        {
            _accesoSQL = accesoSQL ?? throw new ArgumentNullException(nameof(accesoSQL));
        }

        public List<HoraTrabajada> ObtenerHorasTrabajadas(int contratoId, DateTime fechaInicio, DateTime fechaFin)
        {
            var horas = new List<HoraTrabajada>();
            System.Diagnostics.Debug.WriteLine($"Obteniendo horas trabajadas del trabajador ID: {contratoId}");

            try
            {
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("personal.proc_Obtener_Horas_Trabajadas");
                cmd.Parameters.AddWithValue("@contrato_id", contratoId);
                cmd.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fecha_fin", fechaFin);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new HoraTrabajada
                        {
                            Fecha = Convert.ToDateTime(dr["fecha"]),
                            HorasNormales = Convert.ToDecimal(dr["horas_normales"]),
                            HorasExtras = Convert.ToDecimal(dr["horas_extras"]),
                            HorasDescanso = Convert.ToDecimal(dr["horas_descanso"]),
                            TotalDiaTrabajado = Convert.ToDecimal(dr["total_dia_trabajado"]),
                            Contrato = new Contrato
                            {
                                ContratoId = Convert.ToInt32(dr["contrato_id"])
                            }
                        };
                        horas.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                throw new Exception("Error al obtener las horas trabajadas del trabajador", ex);
            }

            return horas;
        }
    }
}
