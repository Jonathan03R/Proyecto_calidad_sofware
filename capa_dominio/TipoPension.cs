using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class TipoPension
    {
        public int tiposPensionId;
        public string nombre;
        public string entidad;
        public double? comisionSobreFlujo;

        public double? ComisionSobreFlujo { get => comisionSobreFlujo; set => comisionSobreFlujo = value; }
        public string Entidad { get => entidad; set => entidad = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public int TipoPensionId { get => tiposPensionId; set => tiposPensionId = value; }


    }
}