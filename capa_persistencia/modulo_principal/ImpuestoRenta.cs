using capa_persistencia.modulo_base;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using static capa_persistencia.modulo_principal.DetallesNomina;

namespace capa_persistencia.modulo_principal
{
    public class ImpuestoRenta
    {
        private readonly AccesoSQLServer _accesoSQL;
       
        public ImpuestoRenta() { _accesoSQL = new AccesoSQLServer(); }

        // C-04: tramos IR por año
        public List<ImpuestoRenta> ObtenerTramosIRPorAnio(int anio)
        {
            var lista = new List<ImpuestoRenta>();
            try
            {
                _accesoSQL.AbrirConexion();
                var cmd = _accesoSQL.ObtenerComandoDeProcedimiento("nomina.proc_obtener_tramos_ir_por_anio");
                cmd.Parameters.AddWithValue("@anio", anio);

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        var tramo = new ImpuestoRenta
                        {
                            TramoId = reader.GetInt32(reader.GetOrdinal("impuesto_renta_tramo_id")),
                            AnioVigencia = reader.GetInt32(reader.GetOrdinal("impuesto_renta_tramo_anio_vigencia")),
                            Numero = reader.GetInt32(reader.GetOrdinal("impuesto_renta_tramo_numero")),
                            LimiteInferiorUIT = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_limite_inferior_uit")),
                            LimiteSuperiorUIT = reader.IsDBNull(reader.GetOrdinal("impuesto_renta_tramo_limite_superior_uit"))
                                                   ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_limite_superior_uit")),
                            LimiteInferiorSoles = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_limite_inferior_soles")),
                            LimiteSuperiorSoles = reader.IsDBNull(reader.GetOrdinal("impuesto_renta_tramo_limite_superior_soles"))
                                                   ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_limite_superior_soles")),
                            TasaPorcentaje = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_tasa_porcentaje")),
                            AcumuladoAnteriorSoles = reader.GetDecimal(reader.GetOrdinal("impuesto_renta_tramo_acumulado_anterior_soles"))
                        };
                        lista.Add(tramo);
                    }
            }
            catch (Exception)
            {
                throw new ExcepcionNomina(ExcepcionNomina.ERROR_DE_CONSULTA);
            }
            finally { _accesoSQL.CerrarConexion(); }
            return lista;
        }
    }
}
