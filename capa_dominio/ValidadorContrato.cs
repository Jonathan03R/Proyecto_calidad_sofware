using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public static class ValidadorContrato
    {
        public static void ValidarFechas(DateTime inicio, DateTime? fin)
        {
            if (fin != null && fin < inicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        public static void ValidarDuracion(DateTime inicio, DateTime fin)
        {
            var duracion = (fin - inicio).TotalDays;
            if (duracion < 1)
                throw new Exception("El contrato debe tener una duración mínima de 1 día.");
        }

        public static void ValidarActivo(bool activo)
        {
            if (!activo)
                throw new Exception("El contrato no se encuentra activo.");
        }
    }
}
