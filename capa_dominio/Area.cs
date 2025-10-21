using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Area
    {
        
        private int areaId;
        private string areaNombre;
        private string areaDescripcion;
        private string sedeNombre; 
        private string sedeDireccion; 
        private string sedeDepartamento; 
        private string sedeProvincia; 

        
        public int AreaId { get => areaId; set => areaId = value; }
        public string AreaNombre { get => areaNombre; set => areaNombre = value; }
        public string AreaDescripcion { get => areaDescripcion; set => areaDescripcion = value; }
        public string SedeNombre { get => sedeNombre; set => sedeNombre = value; } 
        public string SedeDireccion { get => sedeDireccion; set => sedeDireccion = value; } 
        public string SedeDepartamento { get => sedeDepartamento; set => sedeDepartamento = value; } 
        public string SedeProvincia { get => sedeProvincia; set => sedeProvincia = value; } 


    }
}
