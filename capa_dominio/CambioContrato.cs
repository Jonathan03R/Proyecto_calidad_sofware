using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class CambioContrato
    {
        public int CambioContratoId { get; set; }
        public int ContratoId { get; set; }
        public string CampoModificado { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
        public string UsuarioResponsable { get; set; }
        public DateTime FechaCambio { get; set; }

        public string GenerarResumen()
        {
            return $"{FechaCambio:dd/MM/yyyy HH:mm} - {UsuarioResponsable} modificó {CampoModificado} de '{ValorAnterior}' a '{ValorNuevo}'";
        }
    }
}
