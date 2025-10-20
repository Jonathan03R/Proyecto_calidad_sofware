using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class HoraTrabajada
    {
        // ======== Atributos privados ==========
        private int horaTrabajadaId;
        private Contrato contrato;
        private DateTime horaTrabajadaFecha;
        private TimeSpan? horaTrabajadaHoraEntrada;
        private TimeSpan? horaTrabajadaHoraSalida;
        private decimal horasTrabajadas;
        private decimal horasExtra;
        private decimal horaTrabajadaDescanso;
        private string horaTrabajadaObservaciones;
        private DateTime registroFechaCreacion;

        // ======== Propiedades públicas ==========
        public int HoraTrabajadaId { get => horaTrabajadaId; set => horaTrabajadaId = value; }
        public Contrato Contrato { get => contrato; set => contrato = value; }
        public DateTime HoraTrabajadaFecha { get => horaTrabajadaFecha; set => horaTrabajadaFecha = value; }
        public TimeSpan? HoraTrabajadaHoraEntrada { get => horaTrabajadaHoraEntrada; set => horaTrabajadaHoraEntrada = value; }
        public TimeSpan? HoraTrabajadaHoraSalida { get => horaTrabajadaHoraSalida; set => horaTrabajadaHoraSalida = value; }
        public decimal HorasTrabajadas { get => horasTrabajadas; set => horasTrabajadas = value; }
        public decimal HorasExtra { get => horasExtra; set => horasExtra = value; }
        public decimal HoraTrabajadaDescanso { get => horaTrabajadaDescanso; set => horaTrabajadaDescanso = value; }
        public string HoraTrabajadaObservaciones { get => horaTrabajadaObservaciones; set => horaTrabajadaObservaciones = value; }
        public DateTime RegistroFechaCreacion { get => registroFechaCreacion; set => registroFechaCreacion = value; }

        // ======== Métodos de negocio ==========
        public decimal CalcularHorasTotales()
        {
            return horasTrabajadas + horasExtra - horaTrabajadaDescanso;
        }

        public bool EsDiaLaboral()
        {
            // Considera como día laboral de lunes (1) a sábado (6)
            return horaTrabajadaFecha.DayOfWeek != DayOfWeek.Sunday;
        }

        public bool TieneHorasExtras()
        {
            return horasExtra > 0;
        }

        public bool EsRegistroCompleto()
        {
            return horaTrabajadaHoraEntrada.HasValue && horaTrabajadaHoraSalida.HasValue;
        }

        public string Resumen()
        {
            return $"{horaTrabajadaFecha.ToShortDateString()} - {horasTrabajadas} h (+{horasExtra} extra, -{horaTrabajadaDescanso} descanso)";
        }
    }
}
