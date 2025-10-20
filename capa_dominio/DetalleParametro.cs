using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class DetalleParametro
    {
        
        private int detalleParametroId;
        private Parametro parametro;          
        private Trabajador trabajador;       
        private decimal detalleParametroValor;
        private DateTime detalleParametroAplicaDesde;
        private DateTime? detalleParametroAplicaHasta;
        private string detalleParametroObservaciones;

        
        public int DetalleParametroId { get => detalleParametroId; set => detalleParametroId = value; }
        public Parametro Parametro { get => parametro; set => parametro = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public decimal DetalleParametroValor { get => detalleParametroValor; set => detalleParametroValor = value; }
        public DateTime DetalleParametroAplicaDesde { get => detalleParametroAplicaDesde; set => detalleParametroAplicaDesde = value; }
        public DateTime? DetalleParametroAplicaHasta { get => detalleParametroAplicaHasta; set => detalleParametroAplicaHasta = value; }
        public string DetalleParametroObservaciones { get => detalleParametroObservaciones; set => detalleParametroObservaciones = value; }

        
        public bool EstaVigente()
        {
            DateTime hoy = DateTime.Now;
            return detalleParametroAplicaDesde <= hoy &&
                   (!detalleParametroAplicaHasta.HasValue || detalleParametroAplicaHasta.Value >= hoy);
        }

        public bool EsValorValido()
        {
            return detalleParametroValor > 0;
        }

        public string DescripcionCompleta()
        {
            string trabajadorNombre = trabajador != null ? trabajador.Persona.NombreCompleto() : "General";
            return $"{parametro.ParametroNombre} ({parametro.ParametroCodigo}) - {trabajadorNombre}: {detalleParametroValor}";
        }
    }
}
