using capa_aplicacion.Servicios;
using capa_dominio.dto;
using System;
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

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult ListarActivos()
        {
            try
            {
                var data = servicio.ListarContratosActivos();
                var resultado = data.Select(x => new
                {
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

        [HttpGet]
        public JsonResult ListarSinContrato()
        {
            try
            {
                var data = servicio.ListarSinContratoActivo();
                var resultado = data.Select(x => new
                {
                    TrabajadorId = x.TrabajadorId,
                    EmpleadoNombre = x.EmpleadoNombre,
                    Documento = x.Documento,
                    EstadoContratoNombre = x.EstadoContratoNombre
                });
                return Json(new { consultaExitosa = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerDatosNuevoContrato(int trabajadorId)
        {
            try
            {
                var data = servicio.ObtenerDatosParaNuevoContrato(trabajadorId);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
