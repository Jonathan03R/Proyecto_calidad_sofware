using capa_aplicacion.servicios;
using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class NominaController : Controller
{
    private readonly NominasServicios _servicio;
    private readonly ImpuestoRenta _repoImpuestoRenta;
    private readonly Parametros _repoParametros;
    private readonly ReporteService _reporteService;
    private readonly PeriodoService _periodoService;

    public NominaController()
    {
        _servicio = new NominasServicios();
        _repoImpuestoRenta = new ImpuestoRenta();
        _repoParametros = new Parametros();
        _reporteService = new ReporteService();
        _periodoService = new PeriodoService(); 
    }

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Historial()
    {
        var historial = _servicio.ListarDetallesNominasProcesadas();
        return View(historial);
    }

    [HttpGet]
    public JsonResult ListarHistorialPaginado(
    int page = 1,
    int pageSize = 10,
    int? periodoId = null,
    string estado = null,
    string buscar = null)
    {
        try
        {
            var data = _servicio.ListarHistorialPaginado(
                page,
                pageSize,
                periodoId,
                estado,
                buscar
            );

            return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    public JsonResult ListarPeriodosHistorial()
    {
        var periodos = _periodoService.ListarProcesados();
        return Json(new { ok = true, data = periodos }, JsonRequestBehavior.AllowGet);
    }



    // ========== PERIODOS =============
    [HttpGet]
    public JsonResult ListarPeriodos()
    {
        bool accionExitosa;
        string mensajeRetorno;
        List<Periodo> listaPeriodos = new List<Periodo>();

        try
        {
            System.Diagnostics.Debug.WriteLine("[ListarPeriodos] Inicio de consulta");

            listaPeriodos = _periodoService.ListarAbiertos();

            System.Diagnostics.Debug.WriteLine("[ListarPeriodos] Cantidad de periodos obtenidos: " + (listaPeriodos?.Count ?? 0));

            accionExitosa = true;
            mensajeRetorno = "";
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("[ListarPeriodos] ERROR: " + e.Message);

            listaPeriodos = null;
            accionExitosa = false;
            mensajeRetorno = e.Message;
        }

        System.Diagnostics.Debug.WriteLine("[ListarPeriodos] Fin de consulta");

        return Json(new
        {
            data = listaPeriodos,
            consultaExitosa = accionExitosa,
            mensaje = mensajeRetorno
        }, JsonRequestBehavior.AllowGet);
    }

    // ========== PROCESAR NÓMINA =============
    [HttpPost]
    public JsonResult ProcesarNomina(int periodoId)
    {
        try
        {
            if (periodoId <= 0)
                return Json(new { ok = false, msg = "Periodo inválido" });

            int anio = DateTime.Now.Year;

            var tramos = _repoImpuestoRenta.ObtenerTramosIRPorAnio(anio);
            if (tramos == null || tramos.Count == 0)
                return Json(new { ok = false, msg = "No existen tramos IR." });

            var parametros = _repoParametros.ListarParametrosVigentesParaNomina();
            var parametroEssalud = parametros.FirstOrDefault(p => p.ParametroCodigo == "APORTE_ESSALUD");
            var parametroUIT = parametros.FirstOrDefault(p => p.ParametroCodigo.StartsWith("UIT"));

            if (parametroEssalud == null || parametroUIT == null)
                return Json(new { ok = false, msg = "Faltan parámetros ESSALUD o UIT." });

            _servicio.ProcesarNominaPorPeriodo(periodoId, tramos, parametroEssalud, parametroUIT.ParametroValor);

            return Json(new { ok = true, msg = "Nómina procesada correctamente." });
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, msg = ex.Message });
        }
    }

    // ========== DETALLES PROCESADOS =============
    [HttpGet]
    public JsonResult ObtenerDetalleNominasProcesadas(int? periodoId)
    {
        bool accionExitosa;
        string mensajeRetorno;
        List<NominasProcesadasDTO> listaDetalles;

        try
        {
            listaDetalles = _servicio.ListarDetallesNominasProcesadas(
                trabajadorId: null,
                nominaId: null,
                periodoId: periodoId,
                estadoNomina: null
            );
            accionExitosa = true;
            mensajeRetorno = "";
        }
        catch (Exception e)
        {
            listaDetalles = null;
            accionExitosa = false;
            mensajeRetorno = e.Message;
        }

        return Json(new { data = listaDetalles, consultaExitosa = accionExitosa, mensaje = mensajeRetorno },
            JsonRequestBehavior.AllowGet);
    }

}
