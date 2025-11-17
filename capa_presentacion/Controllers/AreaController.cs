using capa_aplicacion.sevicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class AreaController : Controller
{
    private readonly AreaService areaService = new AreaService();

    [HttpGet]
    public JsonResult ObtenerAreas()
    {
        try
        {
            var lista = areaService.ObtenerAreas();
            var dto = (lista ?? new List<capa_dominio.Area>())
                      .Select(a => new { a.AreaId, a.AreaNombre });
            return Json(new { consultaExitosa = true, data = dto }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}
