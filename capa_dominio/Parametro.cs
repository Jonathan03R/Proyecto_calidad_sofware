using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Parametro
    {
       
        private int parametroId;
        private string parametroCodigo;
        private string parametroNombre;
        private decimal parametroValor;
        private string parametroDescripcion;
        private DateTime parametroFechaVigencia;
        private string parametroEstado;

        
        public int ParametroId { get => parametroId; set => parametroId = value; }
        public string ParametroCodigo { get => parametroCodigo; set => parametroCodigo = value; }
        public string ParametroNombre { get => parametroNombre; set => parametroNombre = value; }
        public decimal ParametroValor { get => parametroValor; set => parametroValor = value; }
        public string ParametroDescripcion { get => parametroDescripcion; set => parametroDescripcion = value; }
        public DateTime ParametroFechaVigencia { get => parametroFechaVigencia; set => parametroFechaVigencia = value; }
        public string ParametroEstado { get => parametroEstado; set => parametroEstado = value; }

       
        public bool EsActivo()
        {
            return parametroEstado == "Activo";
        }

        public bool EstaVigente()
        {
            return parametroFechaVigencia <= DateTime.Now;
        }

        public bool EsValido()
        {
            return !string.IsNullOrEmpty(parametroCodigo) &&
                   !string.IsNullOrEmpty(parametroNombre) &&
                   parametroValor > 0;
        }

        public override string ToString()
        {
            return $"{parametroCodigo} - {parametroNombre}: {parametroValor}";
        }
    }
}