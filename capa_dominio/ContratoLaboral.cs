using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class ContratoLaboral : DominioBase
    {
        public int ContratoId { get; set; }
        public int TrabajadorId { get; set; }
        public int CargoId { get; set; }
        public int TipoPensionId { get; set; }
        public int TipoSalarioId { get; set; }
        public int TipoJornadaId { get; set; }
        public int EstadoContratoId { get; set; }

        public decimal Salario { get; set; }
        public decimal? TarifaHora { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaFirma { get; set; }

        public string Modalidad { get; set; }
        public string ModoPago { get; set; }
        public string DocumentoUrl { get; set; }
        public string Observaciones { get; set; }

        private readonly AccesoSQLServer _db = new AccesoSQLServer();

        // RN-01: Validar un solo contrato activo
        public void ValidarContratoUnicoActivo(bool existeContratoActivo)
        {
            if (existeContratoActivo)
                throw new Exception("El trabajador ya posee un contrato activo.");
        }

        // RN-02: Validar datos obligatorios
        public void ValidarCamposObligatorios()
        {
            ValidarMontoPositivo("Salario", Salario);
            if (FechaInicio == DateTime.MinValue)
                throw new Exception("Debe ingresar una fecha de inicio válida.");
        }

        // RN-03: Finalizar contrato
        public void FinalizarContrato(string motivo)
        {
            if (EstadoContratoId != 1)
                throw new Exception("Solo los contratos activos pueden ser finalizados.");

            FechaFin = DateTime.Now;
            Observaciones = motivo;
            EstadoContratoId = 4; // 4 = Finalizado
        }

        // RN-04: Eliminación lógica
        public void MarcarComoInactivo()
        {
            EstadoContratoId = 2; // 2 = Inactivo
        }

        // RN-05: Consultar contratos finalizados (solo lectura)
        public bool EsSoloLectura()
        {
            return EstadoContratoId == 4; 
        }

        // RN-06: Validar salario mayor a la RMV
        public void ValidarSalario(decimal rmv)
        {
            if (Salario < rmv)
                throw new Exception($"El salario ({Salario:C}) no puede ser menor que la RMV ({rmv:C}).");
        }

        // RN-07: Validar que el contrato esté firmado antes del inicio
        public void ValidarFirma()
        {
            if (FechaFirma == null)
                throw new Exception("El contrato debe estar firmado antes del inicio de labores.");
            if (FechaFirma > FechaInicio)
                throw new Exception("La fecha de firma no puede ser posterior al inicio de labores.");
        }

        // RN-08: Validar conservación del documento firmado
        public void ValidarDocumentoAdjunto()
        {
            if (string.IsNullOrWhiteSpace(DocumentoUrl))
                throw new Exception("Debe adjuntar el documento firmado del contrato.");
        }

        // RN-09: Registrar cambios (trazabilidad)
        public CambioContrato RegistrarCambio(string campo, string valorAnterior, string valorNuevo, string usuario)
        {
            return new CambioContrato
            {
                ContratoId = ContratoId,
                CampoModificado = campo,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo,
                UsuarioResponsable = usuario,
                FechaCambio = DateTime.Now
            };
        }

        // RN-10: Buscar trabajadores (método auxiliar)
        public string BuscarPorCriterio(string criterio)
        {
            return $"SELECT * FROM personal.trabajadores WHERE trabajador_codigo LIKE '%{criterio}%' OR persona_id IN (SELECT persona_id FROM personal.personas WHERE persona_nombre LIKE '%{criterio}%');";
        }
    }
}
