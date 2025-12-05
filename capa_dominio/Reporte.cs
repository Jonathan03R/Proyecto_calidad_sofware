using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Reporte
    {
        public Trabajador Trabajador { get; set; }
        public TipoIdentificacion TipoIdentificacion { get; set; }
        public Cargo Cargo { get; set; }
        public Contrato Contrato { get; set; }
        public Nomina Nomina { get; set; }
        public DetalleNomina DetalleNomina { get; set; }
        public Periodo Periodo { get; set; }
    }
}
