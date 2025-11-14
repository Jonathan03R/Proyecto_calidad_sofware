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

        // 🔹 Propiedad privada para el valor hora calculado
        private decimal valorHoraCalculado;
        public decimal ValorHoraCalculado
        {
            get => valorHoraCalculado;
            private set => valorHoraCalculado = value;
        }


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

        public bool EsActivo()
        {
            return EstadoiId == 1;
        }

        public bool EsPorHora()
        {
            return tipoSalario != null &&
                   tipoSalario.TipoSalarioNombre.ToLower().Contains("hora");
        }


        public void CalcularValorPorHora()
        {
            decimal resultado = 0m;

            if (contratoTarifaHora > 0)
            {
                resultado = contratoTarifaHora;
            }
            else if (contratoSalario > 0)
            {
                // // Fórmula usada en tu BD → salario mensual / 240 horas
                resultado = Math.Round(contratoSalario / 240m, 2);
            }

            ValorHoraCalculado = resultado;
        }
    }
}