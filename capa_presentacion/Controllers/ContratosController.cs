using System;
using System.Collections.Generic;
using System.Web.Mvc;
using capa_aplicacion.Servicios;
using capa_dominio.dto;

namespace capa_presentacion.Controllers
{
    public class ContratosController : Controller
    {
        private readonly ServicioContratos servicio;

        public ContratosController()
        {
            servicio = new ServicioContratos();
        }

        // ✅ Página principal: muestra los listados
        public ActionResult Index()
        {
            try
            {
                // No necesitamos cargar las listas aquí, se cargarán con AJAX
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }

        // ✅ Método para obtener trabajadores (JSON)
        [HttpGet]
        public JsonResult ObtenerTrabajadores()
        {
            try
            {
                var trabajadores = servicio.ObtenerTrabajadores();
                return Json(new { success = true, data = trabajadores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ✅ Método para obtener áreas (JSON)
        [HttpGet]
        public JsonResult ObtenerAreas()
        {
            try
            {
                var areas = servicio.ObtenerAreas();
                return Json(new { success = true, data = areas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ✅ Método para obtener cargos (JSON)
        [HttpGet]
        public JsonResult ObtenerCargos()
        {
            try
            {
                var cargos = servicio.ObtenerCargos();
                return Json(new { success = true, data = cargos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ✅ Método para obtener tipos de pensión (JSON)
        [HttpGet]
        public JsonResult ObtenerTiposPension()
        {
            try
            {
                var tipos = servicio.ObtenerTiposPension();
                return Json(new { success = true, data = tipos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ✅ Método para obtener estados de contrato (JSON)
        [HttpGet]
        public JsonResult ObtenerEstadosContrato()
        {
            try
            {
                var estados = servicio.ObtenerEstadosContrato();
                return Json(new { success = true, data = estados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}