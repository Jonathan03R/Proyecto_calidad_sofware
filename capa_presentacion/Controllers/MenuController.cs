using capa_presentacion.Models.Mock;
using System.Collections.Generic;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class MenuController : Controller
    {
        public PartialViewResult Navbar()
        {
            var menu = new List<MenuItem>
            {


                new MenuItem
                {
                    Titulo = "Contratos",
                    Icono = "~/Content/img/icons/Contratos.svg",
                    Controlador = "Contratos",
                    Accion = "Index"
                },
                new MenuItem
                {
                    Titulo = "Dashboard",
                    Icono = "~/Content/img/icons/Dashboard.svg",
                    Controlador = "Nomina",
                    Accion = "Dashboard"
                },
                new MenuItem
                {
                    Titulo = "Procesar Nómina",
                    Icono = "~/Content/img/icons/Nomina.svg",
                    SubItems = new List<MenuItem>
                    {
                        new MenuItem { Titulo = "Vigentes", Controlador = "Nomina", Accion = "Index" },
                        new MenuItem { Titulo = "Historial", Controlador = "Nomina", Accion = "Historial" }
                    }
                },

                new MenuItem
                {
                    Titulo = "Reportes",
                    Icono = "~/Content/img/icons/Reportes.svg",
                    Controlador = "GenerarReporte",
                    Accion = "Index"
                },

                new MenuItem
                {
                    Titulo = "Parámetros",
                    Icono = "~/Content/img/icons/Parametros.svg",
                    Controlador = "Nomina",
                    Accion = "Parametros"
                }     
                
            };

            return PartialView("_Navbar", menu);
        }
    }
}
