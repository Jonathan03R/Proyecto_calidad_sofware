using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using capa_presentacion.Models.ViewModels;
using capa_presentacion.Models.Mock;

namespace capa_presentacion.Controllers
{
    public class NominaController : Controller
    {
        // Acción principal: pantalla inicial con selector de periodo
        public ActionResult Index()
        {
            var periodos = DemoData.ObtenerPeriodos();
            var parametros = DemoData.ObtenerParametrosOficiales();

            var viewModel = new NominaPeriodoVM
            {
                Periodos = periodos,
                Parametros = parametros
            };

            return View(viewModel);
        }

        // Simula el procesamiento de nómina por periodo
        public ActionResult Procesar(string periodoId)
        {
            var resultado = DemoData.SimularProcesamiento(periodoId);
            return View(resultado);
        }

        // Muestra la lista de nóminas procesadas (historial)
        public ActionResult Historial()
        {
            var historial = DemoData.ObtenerHistorialNominas();
            return View(historial);
        }

        // Muestra los parámetros oficiales (parcial)
        public ActionResult Parametros()
        {
            var parametros = DemoData.ObtenerParametrosOficiales();
            return PartialView("_ModalParametros", parametros);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}