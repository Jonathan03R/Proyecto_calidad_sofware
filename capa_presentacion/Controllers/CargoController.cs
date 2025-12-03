using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_dominio;
using capa_dominio.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class CargoController : Controller
    {
        private readonly CargoService cargoService;

        public CargoController()
        {
            cargoService = new CargoService();
        }

        // GET: Cargo
        public ActionResult ListarCargo()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerCargos()
        {
            try
            {
                var lista = cargoService.ObtenerCargos();
                var dto = (lista ?? new List<Cargo>())
                          .Select(c => new { c.CargoId, c.CargoNombre });
                return Json(new { consultaExitosa = true, data = dto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
