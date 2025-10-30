using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class HoraTrabajada
    {
        private int horaTrabajadaId;
        private DateTime horaTrabajadaFecha;
        private TimeSpan? horaTrabajadaHoraEntrada;
        private TimeSpan? horaTrabajadaHoraSalida;
        private decimal horasTrabajadas;
        private int horasExtra; // va a ser dinero
        private decimal valorHoraExtra;
        private decimal horaTrabajadaDescanso;
        private string horaTrabajadaObservaciones;
        private DateTime registroFechaCreacion;
        private Trabajador trabajador;

        public Trabajador Trabajador { get => trabajador; set => trabajador = value; }
        public int HoraTrabajadaId { get => horaTrabajadaId; set => horaTrabajadaId = value; }
        public DateTime HoraTrabajadaFecha { get => horaTrabajadaFecha; set => horaTrabajadaFecha = value; }
        public TimeSpan? HoraTrabajadaHoraEntrada { get => horaTrabajadaHoraEntrada; set => horaTrabajadaHoraEntrada = value; }
        public TimeSpan? HoraTrabajadaHoraSalida { get => horaTrabajadaHoraSalida; set => horaTrabajadaHoraSalida = value; }
        public int HorasExtra { get => horasExtra; set => horasExtra = value; }
        public decimal HorasTrabajadas { get => horasTrabajadas; set => horasTrabajadas = value; }
        public decimal ValorHoraExtra { get => valorHoraExtra; set => valorHoraExtra = value; }
        public decimal HoraTrabajadaDescanso { get => horaTrabajadaDescanso; set => horaTrabajadaDescanso = value; }
        public string HoraTrabajadaObservaciones { get => horaTrabajadaObservaciones; set => horaTrabajadaObservaciones = value; }
        public DateTime RegistroFechaCreacion { get => registroFechaCreacion; set => registroFechaCreacion = value; }

        public decimal CalcularHorasTotales()
        {
            return horasTrabajadas + horasExtra - horaTrabajadaDescanso;
        }

        public decimal CalculoPagoHorasExtras() 
        {
            decimal tarifaPorHora = trabajador.Contrato.ContratoTarifaHora ;
            decimal tarifaHoraNormal = 10.0m; 
            decimal tarifaHoraExtra = tarifaHoraNormal * 1.5m;
            return horasExtra * tarifaHoraExtra;
        }

        public bool EsDiaLaboral()
        {
            return horaTrabajadaFecha.DayOfWeek != DayOfWeek.Sunday;
        }


        /// <summary>
        /// El cálculo de las horas extras se basa en horasTrabajadas.
        /// Se calcula el tiempo entre la hora de entrada y salida.
        /// Si supera las horasTrabajadas, lo restante son horas extras.
        /// </summary>
        public void CalcularHorasTrabajadas()
        {
             TimeSpan HorasTrabajadas = horaTrabajadaHoraSalida.Value - horaTrabajadaHoraEntrada.Value;
        }
    }
}
