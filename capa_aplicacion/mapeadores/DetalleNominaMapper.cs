using capa_dominio;
using capa_dominio.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.mapeadores
{
    public static class DetalleNominaMapper
    {
        public static DetalleNomina ToEntity(DetalleNominaDTO dto)
        {
            return new DetalleNomina
            {
                SueldoBasico = dto.SueldoBasico,
                AsignacionFamiliar = dto.AsignacionFamiliar,
                HorasExtras = dto.HorasExtras,
                BonosRegulares = dto.BonosRegulares,
                OtrosIngresos = dto.OtrosIngresos,
                DescuentoAdelantos = dto.DescuentoAdelantos,
                DescuentoFaltas = dto.DescuentoFaltas,
            };
        }

        public static void FromEntity(DetalleNomina entity, DetalleNominaDTO dto)
        {
            dto.RemuneracionBruta = entity.RemuneracionBruta;
            dto.AporteEssalud = entity.AporteEssalud;
            dto.AporteOnp = entity.AporteONP;
            dto.DescuentoAfp = entity.DescuentoAFP;
            dto.ImpuestoRentaMensual = entity.ImpuestoRentaMensual;
            dto.TotalIngresos = entity.TotalIngresos;
            dto.TotalDescuentos = entity.TotalDescuentos;
            dto.NetoPagar = entity.NetoPagar;
        }
    }
}
