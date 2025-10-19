using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class DominioBase
    {
        public void ValidarCampoObligatorio(string campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new Exception($"El campo '{campo}' es obligatorio.");
        }

        public void ValidarFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        public void ValidarMontoPositivo(string campo, decimal monto)
        {
            if (monto <= 0)
                throw new Exception($"El campo '{campo}' debe ser mayor a cero.");
        }
    }
}
