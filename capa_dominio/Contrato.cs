using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Contrato
    {

        private int contratoId;
        private Trabajador trabajador;
        private Cargo cargo;
        private Area area;
        private TipoPension tipoPension; 
        private TipoSalario tipoSalario;  
        private TipoJornada tipoJornada; ///sacamos este campo
        private int estadoiId;
        //private EstadoContrato estadoContrato;
        private DateTime contratoFechaInicio;
        private DateTime? contratoFechaFin;
        private int? contratoHorasSemanales;
        private decimal contratoTarifaHora;
        private decimal contratoSalario;
        private string contratoModoPago;
        private string contratoDocumentoUrl;
        private string contratoDescripcionFunciones;
        private string contratoObservaciones;
        private DateTime contratoFechaCreacion;

        // 🔹 Propiedades públicas (con acceso controlado)
        public int ContratoId { get => contratoId; set => contratoId = value; }
        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public Cargo Cargo { get => cargo; set => cargo = value; }
        public Area Area { get => area; set => area = value; }
        public TipoPension TipoPension { get => tipoPension; set => tipoPension = value; }
        public TipoSalario TipoSalario { get => tipoSalario; set => tipoSalario = value; }
        public TipoJornada TipoJornada { get => tipoJornada; set => tipoJornada = value; }
        public int EstadoiId { get => estadoiId; set => estadoiId = value; }
        //public EstadoContrato EstadoContrato { get => estadoContrato; set => estadoContrato = value; }
        public DateTime ContratoFechaInicio { get => contratoFechaInicio; set => contratoFechaInicio = value; }
        public DateTime? ContratoFechaFin { get => contratoFechaFin; set => contratoFechaFin = value; }
        public int? ContratoHorasSemanales { get => contratoHorasSemanales; set => contratoHorasSemanales = value; }
        public decimal ContratoTarifaHora { get => contratoTarifaHora; set => contratoTarifaHora = value; }
        public decimal ContratoSalario
        {
            get => contratoSalario;
            set
            {
                if (value < 0)
                    throw new ArgumentException("El salario no puede ser negativo");
                contratoSalario = value;
            }
        }
        public string ContratoModoPago { get => contratoModoPago; set => contratoModoPago = value; }
        public string ContratoDocumentoUrl { get => contratoDocumentoUrl; set => contratoDocumentoUrl = value; }
        public string ContratoDescripcionFunciones { get => contratoDescripcionFunciones; set => contratoDescripcionFunciones = value; }
        public string ContratoObservaciones { get => contratoObservaciones; set => contratoObservaciones = value; }
        public DateTime ContratoFechaCreacion { get => contratoFechaCreacion; set => contratoFechaCreacion = value; }

        /// <summary>
        /// Determina si el contrato está vigente en función de la fecha actual.
        /// </summary>
        /// <remarks>
        /// ⚠️ **Advertencia:** Este método asume que la nómina se procesa en tiempo real (fecha actual).
        /// Si se ejecuta una nómina de un periodo pasado, podría marcar el contrato como *no vigente*
        /// incluso cuando sí lo estaba durante el mes que se está procesando.
        ///
        /// Ejemplo del problema:
        /// - El contrato terminó el 31/10/2025.
        /// - Hoy es 20/11/2025.
        /// - Se procesa la nómina de octubre (periodo anterior).
        /// - Este método devolverá <c>false</c> porque usa <see cref="DateTime.Now"/>,
        ///   y el contrato ya está vencido al momento del procesamiento.
        ///
        /// 💡 **Solución sugerida:** Evaluar la vigencia del contrato comparando con la fecha del periodo
        /// que se está procesando (por ejemplo, el último día del periodo), no con la fecha actual.
        /// </remarks>
        /// <returns>
        /// <c>true</c> si el contrato no tiene fecha de fin o si la fecha de fin es igual o posterior a hoy.
        /// De lo contrario, <c>false</c>.
        /// </returns>
         

        //public bool EstaVigente()
        //{
        //    return !contratoFechaFin.HasValue || contratoFechaFin.Value >= DateTime.Now;
        //}
        public bool EsActivo()
        {
            return EstadoiId == 1;
        }


        public bool EsPorHora()
        {
            return tipoSalario != null && tipoSalario.TipoSalarioNombre.ToLower().Contains("hora");
        }

    }
}