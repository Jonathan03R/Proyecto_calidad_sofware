using capa_aplicacion.Servicios;
using capa_dominio.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class ContratosController : Controller
    {
        private readonly ServicioContratos servicio;

        public ContratosController()
        {
            servicio = new ServicioContratos();
        }

        // ✅ Página principal
        public ActionResult Index()
        {
            try
            {
                // La data se carga vía AJAX
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }

        // ✅ Nuevo método para listar los contratos de trabajadores
        [HttpGet]
        public JsonResult ListarActivos()
        {
            try
            {
                var data = servicio.ListarContratosActivos(); // <- ahora existe
                var resultado = data.Select(x => new {
                    EmpleadoNombre = x.EmpleadoNombre,
                    Documento = x.Documento,
                    CargoNombre = x.CargoNombre,
                    EstadoContratoNombre = x.EstadoContratoNombre,
                    FechaInicio = x.FechaInicio == DateTime.MinValue ? "" : x.FechaInicio.ToString("yyyy-MM-dd"),
                    FechaFin = x.FechaFin.HasValue ? x.FechaFin.Value.ToString("yyyy-MM-dd") : ""
                });
                return Json(new { consultaExitosa = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
