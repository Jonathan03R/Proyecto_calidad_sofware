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

            // Obtener tramos de impuesto a la renta
            var tramos = _repoImpuestoRenta.ObtenerTramosIRPorAnio(anio);
            if (tramos == null || tramos.Count == 0)
                return Json(new { ok = false, msg = "No existen tramos IR." });

            // Obtener parámetros vigentes
            var parametros = _repoParametros.ListarParametrosVigentesParaNomina();

            var parametroEssalud = parametros.FirstOrDefault(p => p.ParametroCodigo == "APORTE_ESSALUD");
            var parametroUIT = parametros.FirstOrDefault(p => p.ParametroCodigo.StartsWith("UIT"));
            var parametroRMV = parametros.FirstOrDefault(p => p.ParametroCodigo == "RMV" || p.ParametroCodigo == "REMUNERACION_MINIMA_VITAL");

            // Validar que existan los parámetros necesarios
            if (parametroEssalud == null)
                return Json(new { ok = false, msg = "Falta parámetro APORTE_ESSALUD." });

            if (parametroUIT == null)
                return Json(new { ok = false, msg = "Falta parámetro UIT." });

            // Calcular el monto fijo de asignación familiar (10% de RMV)
            decimal valorRMV = parametroRMV?.ParametroValor ?? 1130m;
            decimal montoAsignacionFamiliar = Math.Round(valorRMV * 0.10m, 2); // S/ 113 en 2025

            // Llamar al servicio con todos los parámetros
            _servicio.ProcesarNominaPorPeriodo(
                periodoId,
                tramos,
                parametroEssalud,
                parametroUIT.ParametroValor,
                montoAsignacionFamiliar  // ✅ Pasar el MONTO FIJO (S/ 113)
            );

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

    [HttpGet]
    public JsonResult ObtenerEmpleadosVigentesPorPeriodo(int periodoId)
    {
        bool accionExitosa;
        string mensajeRetorno;
        List<ContratoPorPeriodoDTO> listaContratos;

        try
        {
            listaContratos = _servicio.ListarContratosPorPeriodo(periodoId);

            accionExitosa = true;
            mensajeRetorno = "";
        }
        catch (Exception e)
        {
            listaContratos = null;
            accionExitosa = false;
            mensajeRetorno = e.Message;
        }

        return Json(
            new
            {
                data = listaContratos,
                consultaExitosa = accionExitosa,
                mensaje = mensajeRetorno
            },
            JsonRequestBehavior.AllowGet
        );
    }

    [HttpGet]
    public JsonResult ListarResumenNominas()
    {
        bool ok;
        string mensaje;
        List<ResumenNominaDTO> lista;

        try
        {
            lista = _servicio.ListarResumenNominas();
            ok = true;
            mensaje = "";
        }
        catch (Exception ex)
        {
            lista = null;
            ok = false;
            mensaje = ex.Message;
        }

        return Json(new
        {
            data = lista,
            consultaExitosa = ok,
            mensaje = mensaje
        }, JsonRequestBehavior.AllowGet);
    }

}