using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class AuditoriaContrato
    {
        public int ContratoId { get; set; }
        public List<CambioContrato> Cambios { get; set; } = new List<CambioContrato>();

        public void RegistrarCambio(CambioContrato cambio)
        {
            Cambios.Add(cambio);
        }

        public void MostrarHistorial()
        {
            foreach (var c in Cambios)
                Console.WriteLine(c.GenerarResumen());
        }
    }
}
