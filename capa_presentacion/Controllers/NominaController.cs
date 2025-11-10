using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using capa_presentacion.Models.ViewModels;
using capa_presentacion.Models.Mock;
using capa_aplicacion.servicios;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_principal;
using capa_aplicacion.Servicios;


namespace capa_presentacion.Controllers
{
    public class NominaController : Controller
    {
        private readonly NominasServicios _servicio;
        private readonly ImpuestoRenta _repoImpuestoRenta;
        private readonly Parametros _repoParametros;


        public ActionResult Index()
        {
            var model = new NominaPeriodoVM(); // aunque esté vacío, debe existir
            return View(model);
        }
        public NominaController()
        {
            _servicio = new NominasServicios();
            _repoImpuestoRenta = new ImpuestoRenta();
            _repoParametros = new Parametros();
        }

        // Pantalla principal con selector de periodo


        public JsonResult ListarPeriodos()
        {
            var srv = new ReporteService();
            var lista = srv.ListarPeriodos();

            var resultado = lista.Select(x => new PeriodoDTO
            {
                Id = x.PeriodoId,
                Nombre = x.PeriodoNombre
            }).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult ProcesarNomina(int periodoId)
        {
            try
            {
                if (periodoId <= 0)
                    return Json(new { ok = false, msg = "Periodo inválido" });

                int anioActual = DateTime.Now.Year;

                // Obtener tramos IR
                List<ImpuestoRentaTramo> tramos = _repoImpuestoRenta.ObtenerTramosIRPorAnio(anioActual);
                if (tramos == null || !tramos.Any())
                    return Json(new { ok = false, msg = "No existen tramos de impuesto a la renta para el año." });

                // Obtener parámetros
                var parametros = _repoParametros.ListarParametrosVigentesParaNomina();
                var parametroEssalud = parametros.Find(p => p.ParametroCodigo == "APORTE_ESSALUD");
                var parametroUIT = parametros.Find(p => p.ParametroCodigo == "UIT_2025");

                if (parametroEssalud == null || parametroUIT == null)
                    return Json(new { ok = false, msg = "Falta APORTE ESSALUD o UIT" });

                // Procesar nómina
                _servicio.ProcesarNominaPorPeriodo(periodoId, tramos, parametroEssalud, parametroUIT.ParametroValor);

                return Json(new { ok = true, msg = "Nómina generada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, msg = $"Error al procesar la nómina: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult DiagnosticarTrabajadores()
        {
            try
            {
                Console.WriteLine("🔍 Iniciando diagnóstico de trabajadores...");

                var servicio = new NominasServicios();

                // Usar reflexión para acceder al método privado de prueba
                var trabajadoresField = typeof(NominasServicios).GetField("_trabajadores",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var trabajadoresRepo = trabajadoresField.GetValue(servicio) as TrabajadoresRepositorio;

                // Probar obtener trabajadores
                var trabajadores = trabajadoresRepo.ObtenerEmpleados();

                return Json(new
                {
                    ok = true,
                    totalTrabajadores = trabajadores.Count,
                    trabajadoresConContrato = trabajadores.Count(t => t.Contrato != null),
                    trabajadoresSinContrato = trabajadores.Count(t => t.Contrato == null),
                    detalles = trabajadores.Select(t => new {
                        id = t.TrabajadorId,
                        nombre = $"{t.Nombres} {t.Apellidos}",
                        estado = t.Estado,
                        tieneContrato = t.Contrato != null,
                        salario = t.Contrato?.ContratoSalario
                    }).Take(10) // Solo primeros 10 para no saturar
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en diagnóstico: {ex.Message}");
                return Json(new
                {
                    ok = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

    }
}