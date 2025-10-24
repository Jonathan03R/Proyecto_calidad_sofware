using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{

    /// <summary>
    ///  esta clase en la base de datos solo almacena datos como pendienten cancelado etc etc 
    /// </summary>
    public class EstadoContrato
    {
        private int estadoContratoId;
        private string estadoContratoNombre;
        private char estadoContratoEstado;
        private DateTime estadoContratoFechaCreacion;

        
        public int EstadoContratoId { get => estadoContratoId; set => estadoContratoId = value; }
        public string EstadoContratoNombre { get => estadoContratoNombre; set => estadoContratoNombre = value; }
        public char EstadoContratoEstado { get => estadoContratoEstado; set => estadoContratoEstado = value; }
        public DateTime EstadoContratoFechaCreacion { get => estadoContratoFechaCreacion; set => estadoContratoFechaCreacion = value; }

    }
}
