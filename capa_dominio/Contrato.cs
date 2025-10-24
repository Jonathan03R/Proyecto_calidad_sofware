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
        private EstadoContrato estadoContrato;
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
        public EstadoContrato EstadoContrato { get => estadoContrato; set => estadoContrato = value; }
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


        public bool EstaVigente()
        {
            return !contratoFechaFin.HasValue || contratoFechaFin.Value >= DateTime.Now;
        }

        public bool EsPorHora()
        {
            return tipoSalario != null && tipoSalario.TipoSalarioNombre.ToLower().Contains("hora");
        }

    }
}