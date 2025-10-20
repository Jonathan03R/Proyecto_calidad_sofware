using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
/// <summary>
/// ver si hay proc de tipo pensiones
/// </summary>
    public class TipoPension
    {
       
        private int tipoPensionId;
        private string tipoPensionNombre;
        private string tipoPensionEntidad;
        private char tipoPensionEstado;
        private DateTime tipoPensionFechaCreacion;

        
        public int TipoPensionId { get => tipoPensionId; set => tipoPensionId = value; }
        public string TipoPensionNombre { get => tipoPensionNombre; set => tipoPensionNombre = value; }
        public string TipoPensionEntidad { get => tipoPensionEntidad; set => tipoPensionEntidad = value; }
        public char TipoPensionEstado { get => tipoPensionEstado; set => tipoPensionEstado = value; }
        public DateTime TipoPensionFechaCreacion { get => tipoPensionFechaCreacion; set => tipoPensionFechaCreacion = value; }

        
        public bool EstaActiva()
        {
            return tipoPensionEstado == 'A' || tipoPensionEstado == 'a';
        }

        public void Activar()
        {
            tipoPensionEstado = 'A';
        }

        public void Desactivar()
        {
            tipoPensionEstado = 'I';
        }

        public string Resumen()
        {
            string estado = EstaActiva() ? "Activa" : "Inactiva";
            return $"{tipoPensionNombre} - {tipoPensionEntidad} ({estado})";
        }

    }
}
