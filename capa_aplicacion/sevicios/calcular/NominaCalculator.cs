using capa_dominio;
using capa_dominio.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_aplicacion.sevicios.calcular
{
    public class NominaCalculator : INominaCalcular
    {
        public DetalleNomina Calcular(
            Contrato contrato,
            Periodo periodo,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            List<HoraTrabajada> horas,
            List<TipoHoraExtra> tiposExtras,
            bool tieneHijos
        )
        {
            var detalle = new DetalleNomina
            {
                Nomina = new Nomina { Periodo = periodo },
                Contrato = contrato,
                SueldoBasico = contrato.ContratoSalario,
                HorasTrabajadas = horas,
                TiposHorasExtras = tiposExtras,
                BonosRegulares = 0,
                OtrosIngresos = 0
            };

            detalle.CalculoAsignacionFamiliar(tieneHijos);
            detalle.CalcularPagoTotalHorasExtras();
            detalle.CalcularDescuentoTardanzas();
            detalle.CalcularDescuentoFaltas();
            detalle.CalcularRemuneracionBruta();
            detalle.CalcularSistemaPensiones();
            detalle.CalcularAporteEssalud(parametroEssalud);
            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);
            detalle.CalcularTotales();

            return detalle;
        }
    }

}
