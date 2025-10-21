using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Sede
    {
        public char SedeEstado;
        private int sedeId;
        private string sedeNombre;
        private string sedeDireccion;
        private string sedeDepartamento;
        private string sedeProvincia;
        private List<Area> areas = new List<Area>();


        public int SedeId { get => sedeId; set => sedeId = value; }
        public string SedeNombre { get => sedeNombre; set => sedeNombre = value; }
        public string SedeDireccion { get => sedeDireccion; set => sedeDireccion = value; }
        public string SedeDepartamento { get => sedeDepartamento; set => sedeDepartamento = value; }
        public string SedeProvincia { get => sedeProvincia; set => sedeProvincia = value; }
        public List<Area> Areas { get => areas; set => areas = value; }

    }
}

