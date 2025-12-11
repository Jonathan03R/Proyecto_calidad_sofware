using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.interfaces
{
    public interface INominaCalcular
    {
        DetalleNomina Calcular(
            Contrato contrato,
            Periodo periodo,
            List<ImpuestoRentaTramo> tramos,
            Parametro parametroEssalud,
            decimal valorUIT,
            List<HoraTrabajada> horas,
            List<TipoHoraExtra> tiposExtras,
            bool tieneHijos
        );
    }
}
