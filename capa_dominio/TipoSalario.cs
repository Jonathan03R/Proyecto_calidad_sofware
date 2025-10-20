using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoSalario
    {
        
        private int tipoSalarioId;
        private string tipoSalarioNombre;
        private char tipoSalarioEstado;
        private DateTime tipoSalarioFechaCreacion;

        
        public int TipoSalarioId { get => tipoSalarioId; set => tipoSalarioId = value; }
        public string TipoSalarioNombre { get => tipoSalarioNombre; set => tipoSalarioNombre = value; }
        public char TipoSalarioEstado { get => tipoSalarioEstado; set => tipoSalarioEstado = value; }
        public DateTime TipoSalarioFechaCreacion { get => tipoSalarioFechaCreacion; set => tipoSalarioFechaCreacion = value; }

        
        public bool EsActivo()
        {
            return tipoSalarioEstado == 'A';
        }

        public bool EsValido()
        {
            return !string.IsNullOrEmpty(tipoSalarioNombre);
        }
    }
}



