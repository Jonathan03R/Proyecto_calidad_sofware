using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.dto
{

        public class ResultadoEmpleadoDTO
        {
            public int trabajadorId { get; set; }
            public bool ok { get; set; }
            public string mensaje { get; set; }

            public ResultadoEmpleadoDTO(int id, bool estado, string msg)
            {
                trabajadorId = id;
                ok = estado;
                mensaje = msg;
            }
        }

}
