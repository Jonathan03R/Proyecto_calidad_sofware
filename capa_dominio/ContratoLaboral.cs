using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ContratoLaboral
    {
        
        public int ContratoId { get; private set; }
        public int TrabajadorId { get; private set; }
        public int CargoId { get; private set; }
        public int AreaId { get; private set; }
        public int TipoPensionId { get; private set; }
        public int TipoSalarioId { get; private set; }
        public int TipoJornadaId { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime? FechaFin { get; private set; }
        public decimal? Salario { get; private set; }
        public decimal? TarifaHora { get; private set; }
        public string ModoPago { get; private set; }
        public string DocumentoUrl { get; private set; }
        public string DescripcionFunciones { get; private set; }
        public string Observaciones { get; private set; }
        public string Estado { get; private set; }

        
        public ContratoLaboral(int trabajadorId, int cargoId, int areaId, int tipoPensionId, int tipoSalarioId,
            int tipoJornadaId, DateTime fechaInicio, DateTime? fechaFin = null, decimal? salario = null, decimal? tarifaHora = null,
            string modoPago = "Transferencia", string documentoUrl = null, string descripcion = null, string observaciones = null)
        {
            TrabajadorId = trabajadorId;
            CargoId = cargoId;
            AreaId = areaId;
            TipoPensionId = tipoPensionId;
            TipoSalarioId = tipoSalarioId;
            TipoJornadaId = tipoJornadaId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Salario = salario;
            TarifaHora = tarifaHora;
            ModoPago = modoPago;
            DocumentoUrl = documentoUrl;
            DescripcionFunciones = descripcion;
            Observaciones = observaciones;
            Estado = "Activo";

            ValidarContrato();
        }

        // Reglas de negocio 
        public void FinalizarContrato(string motivo)
        {
            if (Estado == "Finalizado")
                throw new InvalidOperationException("El contrato ya está finalizado.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe indicar un motivo de finalización.");

            Estado = "Finalizado";
            FechaFin = DateTime.Now;
            Observaciones = motivo;
        }

        public void SuspenderContrato(string motivo)
        {
            if (Estado == "Suspendido")
                throw new InvalidOperationException("El contrato ya está suspendido.");

            Estado = "Suspendido";
            Observaciones = motivo;
        }

        public void ReactivarContrato()
        {
            if (Estado != "Suspendido" && Estado != "Inactivo")
                throw new InvalidOperationException("Solo se pueden reactivar contratos suspendidos o inactivos.");

            Estado = "Activo";
            Observaciones = "Contrato reactivado el " + DateTime.Now.ToShortDateString();
        }

        public void ValidarContrato()
        {
            if (TrabajadorId <= 0) throw new ArgumentException("Debe asignar un trabajador válido.");
            if (CargoId <= 0) throw new ArgumentException("Debe asignar un cargo válido.");
            if (AreaId <= 0) throw new ArgumentException("Debe asignar un área válida.");
            if (TipoPensionId <= 0) throw new ArgumentException("Debe asignar un sistema de pensión válido.");
            if (TipoSalarioId <= 0) throw new ArgumentException("Debe asignar un tipo de salario válido.");
            if (TipoJornadaId <= 0) throw new ArgumentException("Debe asignar un tipo de jornada válido.");
            if (FechaInicio == DateTime.MinValue) throw new ArgumentException("La fecha de inicio no es válida.");

            if (FechaFin.HasValue && FechaFin <= FechaInicio)
                throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");

            if (Salario.HasValue && Salario < 0)
                throw new ArgumentException("El salario no puede ser negativo.");

            if (TarifaHora.HasValue && TarifaHora < 0)
                throw new ArgumentException("La tarifa por hora no puede ser negativa.");
        }

        public override string ToString()
        {
            return $"Contrato {ContratoId}: Trabajador {TrabajadorId}, Estado: {Estado}, Inicio: {FechaInicio:d}";
        }
    }
}

