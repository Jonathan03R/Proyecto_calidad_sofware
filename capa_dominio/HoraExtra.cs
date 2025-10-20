using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class HoraExtra
    {
        private int horaExtraId;
        private Contrato contrato;
        private HoraTrabajada horaTrabajada;
        private TipoHoraExtra tipoHoraExtra;
        private DateTime horasExtrasFecha;
        private decimal horasExtrasCantidad;
        private decimal horasExtrasTarifaHoraAplicada;
        private decimal multiplicadorAplicado;
        private char estado;
        private string creadoPor;
        private DateTime fechaCreacion;

        
        public int HoraExtraId { get => horaExtraId; set => horaExtraId = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public HoraTrabajada HoraTrabajada { get => horaTrabajada; set => horaTrabajada = value; }
        public TipoHoraExtra TipoHoraExtra { get => tipoHoraExtra; set => tipoHoraExtra = value; }
        public DateTime HorasExtrasFecha { get => horasExtrasFecha; set => horasExtrasFecha = value; }
        public decimal HorasExtrasCantidad { get => horasExtrasCantidad; set => horasExtrasCantidad = value; }
        public decimal HorasExtrasTarifaHoraAplicada { get => horasExtrasTarifaHoraAplicada; set => horasExtrasTarifaHoraAplicada = value; }
        public decimal MultiplicadorAplicado { get => multiplicadorAplicado; set => multiplicadorAplicado = value; }
        public char Estado { get => estado; set => estado = value; }
        public string CreadoPor { get => creadoPor; set => creadoPor = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }

        
        public decimal ValorUnitario
        {
            get => horasExtrasTarifaHoraAplicada * multiplicadorAplicado;
        }

        public decimal Total
        {
            get => horasExtrasCantidad * ValorUnitario;
        }

       
        public bool EstaPendiente()
        {
            return estado == 'P';
        }

        public bool EsValida()
        {
            return horasExtrasCantidad > 0 && horasExtrasTarifaHoraAplicada > 0 && multiplicadorAplicado > 0;
        }

        public void AplicarTipoHoraExtra()
        {
            if (tipoHoraExtra != null)
                multiplicadorAplicado = tipoHoraExtra.TiposHorasExtrasMultiplicador;
        }

        public string Resumen()
        {
            return $"{horasExtrasFecha.ToShortDateString()} - {horasExtrasCantidad} h ({tipoHoraExtra?.TiposHorasExtrasNombre}) = S/. {Total}";
        }
    }
}
