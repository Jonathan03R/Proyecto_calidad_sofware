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

        public int AreaId { get => areaId; set => areaId = value; }
        public string AreaNombre { get => areaNombre; set => areaNombre = value; }
        public string AreaDescripcion { get => areaDescripcion; set => areaDescripcion = value; }
    }
}
