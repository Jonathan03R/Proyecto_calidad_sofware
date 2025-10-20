using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace capa_dominio
{
    public class Contrato
    {
        public int ContratoId { get; set; }
        public int TrabajadorId { get; set; }
        public int CargoId { get; set; }
        public int TipoPensionId { get; set; }
        public int TipoSalarioId { get; set; }
        public int HorarioTrabajoId { get; set; }
        public int EstadoContratoId { get; set; }
        public int SedeId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; } 
        public decimal Salario { get; set; }
        public string ModoPago { get; set; }
        public string DocumentoURL { get; set; }
        public string DescripcionFunciones { get; set; }
        public string Observaciones { get; set; }
        public char Estado { get; set; } = 'A';

        // Relaciones de dominio
        public List<ContratoDetalle> Detalles { get; set; } = new List<ContratoDetalle>();

        // ======== Métodos de negocio =========
        public bool EstaActivo() => Estado == 'A';
        public bool EstaSuspendido() => Estado == 'S';
        public bool EstaFinalizado() => Estado == 'I';
    }
}
