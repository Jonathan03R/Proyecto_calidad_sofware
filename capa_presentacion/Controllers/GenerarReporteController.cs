using capa_aplicacion.Servicios;
using capa_dominio;
using capa_dominio.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class GenerarReporteController : Controller
    {
        private readonly ReporteService reporteService;

        public GenerarReporteController()
        {
            reporteService = new ReporteService();
        }

        // GET: Reporte
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarPeriodos()
        {
            Boolean accionExitosa;
            String mensajeRetorno;
            List<Periodo> listaPeriodos = new List<Periodo>();
            try
            {
                listaPeriodos = reporteService.ListarPeriodos();
                accionExitosa = true;
                mensajeRetorno = "";
            }
            catch (Exception e)
            {
                listaPeriodos = null;
                accionExitosa = false;
                mensajeRetorno = e.Message;
            }

            return Json(new { data = listaPeriodos, consultaExitosa = accionExitosa, mensaje = mensajeRetorno }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult ListarNominaPorPeriodo(int periodoId, int? cargoId = null)
        {
            Boolean accionExitosa;
            String mensajeRetorno;
            List<ReporteNominaDTO> listaReporteNominaPorPeriodo = new List<ReporteNominaDTO>();
            try
            {
                listaReporteNominaPorPeriodo = reporteService.ConsultarNominaPorPeriodo(periodoId, cargoId);
                accionExitosa = true;
                mensajeRetorno = "";
            }
            catch (Exception e)
            {
                listaReporteNominaPorPeriodo = null;
                accionExitosa = false;
                mensajeRetorno = e.Message;
            }

            return Json(new { data = listaReporteNominaPorPeriodo, consultaExitosa = accionExitosa, mensaje = mensajeRetorno }, JsonRequestBehavior.AllowGet);

        }
    }
}
