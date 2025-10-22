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
        private DateTime parametroFechaVigencia;
        private string parametroEstado;

        public int ParametroId { get => parametroId; set => parametroId = value; }
        public string ParametroCodigo { get => parametroCodigo; set => parametroCodigo = value; }
        public string ParametroNombre { get => parametroNombre; set => parametroNombre = value; }
        public decimal ParametroValor { get => parametroValor; set => parametroValor = value; }

        public DateTime ParametroFechaVigencia { get => parametroFechaVigencia; set => parametroFechaVigencia = value; }
        public string ParametroEstado { get => parametroEstado; set => parametroEstado = value; }

        public bool EsVigente()
        {
            return parametroEstado == "Activo" && parametroFechaVigencia <= DateTime.Now;
        }

    }
}

