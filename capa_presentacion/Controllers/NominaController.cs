using capa_aplicacion.servicios;
using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

public class NominaController : Controller
{
    private readonly NominasServicios _servicio;
    private readonly ImpuestoRentaRepositorio _repoImpuestoRenta;
    private readonly ParametrosRepositorio _repoParametros;
    private readonly ReporteService _reporteService;
    private readonly PeriodoService _periodoService;

    public NominaController()
    {
        _servicio = new NominasServicios();
        _repoImpuestoRenta = new ImpuestoRentaRepositorio();
        _repoParametros = new ParametrosRepositorio();
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
        try
        {
            var listaPeriodos = _periodoService.ListarAbiertos();

            // DEBUG:
            Debug.WriteLine("=== PERIODOS DEVUELTOS ===");
            foreach (var p in listaPeriodos)
            {
                Debug.WriteLine($"ID: {p.PeriodoId} | Nombre: {p.PeriodoNombre} | Estado: {p.EstadoId}");
            }
            Debug.WriteLine("=== FIN ===");

            return Json(new { data = listaPeriodos, consultaExitosa = true, mensaje = "" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            return Json(new { data = (object)null, consultaExitosa = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    // -----------------------
    // INICIAR PROCESO -> crea cabecera y devuelve lista de pendientes con nombres
    // -----------------------
    [HttpPost]
    public JsonResult IniciarProceso(int periodoId)
    {
        try
        {
            if (periodoId <= 0)
                return Json(new { ok = false, msg = "Periodo inválido" });

            // ÚNICA llamada: crea nomina y obtiene pendientes dentro de una transacción
            var inicio = _servicio.IniciarProcesoYObtenerPendientes(periodoId);

            var nominaId = inicio.nominaId;
            var pendientesIds = inicio.trabajadoresPendientes;

            // Mapear información completa de los trabajadores
            var contratos = _servicio.ObtenerContratosParaPeriodo(periodoId);
            var trabajadores = contratos
                .Where(c => c.TrabajadorId.HasValue && pendientesIds.Contains(c.TrabajadorId.Value))
                .Select(c => new
                {
                    trabajadorId = c.TrabajadorId.Value,
                    nombre = (c.PersonaNombre ?? string.Empty) + " " + (c.PersonaApellido ?? string.Empty),
                    contratoId = c.ContratoId
                })
                .ToList();

            return Json(new
            {
                ok = true,
                nominaId,
                totalPendientes = trabajadores.Count,
                trabajadores
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, msg = ex.Message });
        }
    }

    // -----------------------
    // PROCESAR TRABAJADOR -> llamado por frontend por cada trabajador
    // -----------------------
    [HttpPost]
    public JsonResult ProcesarTrabajador(int nominaId, int trabajadorId, int periodoId)
    {
        try
        {
            if (nominaId <= 0 || trabajadorId <= 0 || periodoId <= 0)
                return Json(new { ok = false, msg = "Parametros invalidos" });

            // obtener tramos y parametros (se repite en cada llamada; si quieres optimizar,
            // haz que el frontend obtenga estos y los envíe)
            int anio = DateTime.Now.Year;
            var tramos = _repoImpuestoRenta.ObtenerTramosIRPorAnio(anio);
            if (tramos == null || tramos.Count == 0)
                return Json(new { ok = false, msg = "No existen tramos IR." });

            var parametros = _repoParametros.ListarParametrosVigentesParaNomina();
            var parametroEssalud = parametros.FirstOrDefault(p => p.ParametroCodigo == "APORTE_ESSALUD");
            var parametroUIT = parametros.FirstOrDefault(p => p.ParametroCodigo.StartsWith("UIT"));

            if (parametroEssalud == null || parametroUIT == null)
                return Json(new { ok = false, msg = "Faltan parámetros ESSALUD o UIT." });

            var resultado = _servicio.ProcesarTrabajadorEnNomina(nominaId, trabajadorId, periodoId, tramos, parametroEssalud, parametroUIT.ParametroValor);

            return Json(new
            {
                ok = resultado.ok,
                trabajadorId = trabajadorId,
                mensaje = resultado.mensaje,
            });
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, trabajadorId, msg = ex.Message });
        }
    }

    // -----------------------
    // CERRAR PROCESO -> marca estado final de nómina y actualiza periodo
    // acepta huboErrores opcional; si no se envía, calcula por pendientes
    // -----------------------
    [HttpPost]
    public JsonResult CerrarProceso(int nominaId, int periodoId, bool? huboErrores, bool cancelado = false)
    {
        Debug.WriteLine($"[CerrarProceso] INICIO - NominaId: {nominaId}, PeriodoId: {periodoId}, HuboErrores: {huboErrores}, Cancelado:{cancelado}");

        try
        {
            if (nominaId <= 0 || periodoId <= 0)
                return Json(new { ok = false, msg = "Parametros invalidos" });

            _servicio.CerrarNominaYActualizarPeriodo(nominaId, periodoId, huboErrores, cancelado);

            return Json(new { ok = true, msg = cancelado ? "Proceso cancelado" : "Proceso finalizado" });
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, msg = ex.Message });
        }
    }

    // ========== DETALLES y RESUMEN ==========
    [HttpGet]
    public JsonResult ObtenerDetalleNominasProcesadas(int? periodoId)
    {
        try
        {
            var listaDetalles = _servicio.ListarDetallesNominasProcesadas(null, null, periodoId, null);
            return Json(new { data = listaDetalles, consultaExitosa = true, mensaje = "" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            return Json(new { data = (object)null, consultaExitosa = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    public JsonResult ObtenerResumenProcesoNomina(int periodoId)
    {
        try
        {
            if (periodoId <= 0) return Json(new { ok = false, msg = "periodo inválido" }, JsonRequestBehavior.AllowGet);

            var contratos = _servicio.ObtenerContratosParaPeriodo(periodoId);
            var trabajadores = contratos
                .Where(c => c.TrabajadorId.HasValue)
                .Select(c => new
                {
                    trabajadorId = c.TrabajadorId.Value,
                    nombre = (c.PersonaNombre ?? string.Empty) + " " + (c.PersonaApellido ?? string.Empty)
                })
                .ToList();

            return Json(new { ok = true, totalEmpleados = trabajadores.Count, trabajadores }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    public JsonResult ObtenerEmpleadosVigentesPorPeriodo(int periodoId)
    {
        try
        {
            var listaContratos = _servicio.ListarContratosPorPeriodo(periodoId);
            return Json(new { data = listaContratos, consultaExitosa = true, mensaje = "" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            return Json(new { data = (object)null, consultaExitosa = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpGet]
    public JsonResult ListarResumenNominas()
    {
        try
        {
            var lista = _servicio.ListarResumenNominas();
            return Json(new { data = lista, consultaExitosa = true, mensaje = "" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { data = (object)null, consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}
