using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Reporte
    {
        private Trabajador trabajador;
        private TipoIdentificacion tipoIdentificacion;
        private Cargo cargo;
        private Contrato contrato;
        private Nomina nomina;
        private DetalleNomina detalleNomina;
        private Periodo periodo;

        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public TipoIdentificacion TipoIdentificacion { get => tipoIdentificacion; set => tipoIdentificacion = value; }
        public Cargo Cargo { get => cargo; set => cargo = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public Nomina Nomina { get => nomina; set => nomina = value; }
        public DetalleNomina DetalleNomina { get => detalleNomina; set => detalleNomina = value; }
        public Periodo Periodo { get => periodo; set => periodo = value; }


    }
}
