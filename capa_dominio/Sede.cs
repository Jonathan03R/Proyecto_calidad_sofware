using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Sede
    {
        
        private int sedeId;
        private string sedeNombre;
        private string sedeDireccion;
        private string sedeDepartamento;
        private string sedeProvincia;
        private char sedeEstado;
        private DateTime sedeCreacion;

        
        public int SedeId { get => sedeId; set => sedeId = value; }
        public string SedeNombre { get => sedeNombre; set => sedeNombre = value; }
        public string SedeDireccion { get => sedeDireccion; set => sedeDireccion = value; }
        public string SedeDepartamento { get => sedeDepartamento; set => sedeDepartamento = value; }
        public string SedeProvincia { get => sedeProvincia; set => sedeProvincia = value; }
        public char SedeEstado { get => sedeEstado; set => sedeEstado = value; }
        public DateTime SedeCreacion { get => sedeCreacion; set => sedeCreacion = value; }

       
        public bool EstaActiva()
        {
            return sedeEstado == 'A' || sedeEstado == 'a';
        }

        public void Activar()
        {
            sedeEstado = 'A';
        }

        public void Desactivar()
        {
            sedeEstado = 'I';
        }

        public string UbicacionCompleta()
        {
            return $"{sedeDireccion}, {sedeProvincia}, {sedeDepartamento}";
        }

        public string Resumen()
        {
            string estado = EstaActiva() ? "Activa" : "Inactiva";
            return $"{sedeNombre} ({estado}) - {UbicacionCompleta()}";
        }
    }
}

