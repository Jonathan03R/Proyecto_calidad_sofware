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
        public JsonResult ObtenerCargos(int? cargoId = null, string cargoNombre = null)
        {
            Boolean accionExitosa;
            String mensajeRetorno;
            List<Cargo> listaCargo = new List<Cargo>();
            try
            {
                listaCargo = cargoService.ObtenerCargos(cargoId, cargoNombre);
                accionExitosa = true;
                mensajeRetorno = "";
            }
            catch (Exception e)
            {
                listaCargo = null;
                accionExitosa = false;
                mensajeRetorno = e.Message;

                // ✅ Debug adicional
                System.Diagnostics.Debug.WriteLine("Error completo: " + e.ToString());
            }

            return Json(new { data = listaCargo, consultaExitosa = accionExitosa, mensaje = mensajeRetorno }, JsonRequestBehavior.AllowGet);

        }
    }
}
