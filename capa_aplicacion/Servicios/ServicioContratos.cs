using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using capa_dominio.dto;
using capa_persistencia.modulo_principal;

namespace capa_aplicacion.Servicios
{
    public class ServicioContratos
    {
        private readonly Contratos _repo;

        public ServicioContratos()
        {
            _repo = new Contratos();
        }

        // ✅ CREAR CONTRATO
        public int CrearContrato(ContratoDTO contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser nulo.");

            if (contrato.FechaInicio == DateTime.MinValue)
                throw new ArgumentException("Debe especificar una fecha de inicio válida.");

            if (contrato.FechaFin.HasValue && contrato.FechaFin < contrato.FechaInicio)
                throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");

            if ((contrato.Salario ?? 0) <= 0 && (contrato.TarifaHora ?? 0) <= 0)
                throw new ArgumentException("Debe especificar un salario o una tarifa por hora válida.");

            return _repo.CrearContratoEmpleado(contrato);
        }

        // ✅ ACTUALIZAR CONTRATO
        public void ActualizarContrato(int contratoId, string usuario, string motivo, ContratoDTO contrato)
        {
            if (contratoId <= 0)
                throw new ArgumentException("El ID del contrato no es válido.");

            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Debe indicar el usuario que realiza la actualización.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe indicar el motivo de la actualización.");

            _repo.ActualizarContrato(contratoId, usuario, motivo, contrato);
        }

        // ✅ FINALIZAR CONTRATO
        public (int contratoActualizado, int cambioRegistrado) FinalizarContrato(int contratoId, string observaciones = null)
        {
            if (contratoId <= 0)
                throw new ArgumentException("El ID del contrato no es válido.");

            return _repo.FinalizarContrato(contratoId, observaciones);
        }

        // ✅ CONSULTAR CONTRATOS POR TRABAJADOR
        public List<ContratoDTO> ConsultarContratosPorTrabajador(int trabajadorId)
        {
            if (trabajadorId <= 0)
                throw new ArgumentException("El ID del trabajador no es válido.");

            return _repo.ObtenerContratosPorTrabajador(trabajadorId);
        }

        // ✅ CONSULTAR TODOS LOS CONTRATOS
        public List<ContratoDTO> ConsultarTodosLosContratos()
        {
            // Este método llamaría a un SP tipo 'proc_obtener_todos_los_contratos'
            // que devuelva todos los registros de contratos de la empresa
            throw new NotImplementedException("Implementar consulta general de contratos en la capa de persistencia.");
        }
    }
}