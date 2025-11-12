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
    }
}