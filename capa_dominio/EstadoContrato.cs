using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
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

       
        public bool EstaActivo()
        {
            return estadoContratoEstado == 'A' || estadoContratoEstado == 'a';
        }

        public void Activar()
        {
            estadoContratoEstado = 'A';
        }

        public void Desactivar()
        {
            estadoContratoEstado = 'I';
        }

        public bool EsVigente()
        {
            return estadoContratoNombre.ToLower().Contains("vigente") || estadoContratoNombre.ToLower().Contains("activo");
        }

        public bool EsFinalizado()
        {
            return estadoContratoNombre.ToLower().Contains("finalizado") || estadoContratoNombre.ToLower().Contains("concluido");
        }

        public string Resumen()
        {
            string estado = EstaActivo() ? "Activo" : "Inactivo";
            return $"{estadoContratoNombre} ({estado})";
        }
    }
}
