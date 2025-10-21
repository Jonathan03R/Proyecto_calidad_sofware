using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Cargo
    {
        private int cargoId;
        private string cargoNombre;

        public int CargoId
        {
            get => cargoId;
            set => cargoId = value;
        }

        public string CargoNombre
        {
            get => cargoNombre;
            set => cargoNombre = value;
        }
    }
}

