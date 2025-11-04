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
                // Cargar las listas desde la capa de aplicación
                ViewBag.Trabajadores = servicio.ObtenerTrabajadores();
                ViewBag.Areas = servicio.ObtenerAreas();
                ViewBag.Cargos = servicio.ObtenerCargos();
                ViewBag.TiposPension = servicio.ObtenerTiposPension();
                ViewBag.EstadosContrato = servicio.ObtenerEstadosContrato();

                return View();
            }
            catch (Exception ex)
            {
                // 🔹 Muestra información detallada del error en la vista Error
                ViewBag.Error = "Error al cargar los datos: " + ex.Message + " - " + ex.StackTrace;
                return View("Error");
            }
        }
    }
}
