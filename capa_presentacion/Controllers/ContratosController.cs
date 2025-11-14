using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_aplicacion.sevicios.Tipos_salarios;
using capa_dominio;
using System;
using System.Linq;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class ContratosController : Controller
    {
        private readonly ServicioContratos servicio;
        private readonly AreaService _areaService;
        private readonly CargoService _cargoService;
        private readonly PensionService _pensionService;
        private readonly TipoSalarioServicio _tipoSalarioService;
        private readonly TipoJornadaService _jornadaService;

        public ContratosController()
        {
            servicio = new ServicioContratos();
            _areaService = new AreaService();
            _cargoService = new CargoService();
            _pensionService = new PensionService();
            _tipoSalarioService = new TipoSalarioServicio();
            _jornadaService = new TipoJornadaService();
        }

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult ListarActivos()
        {
            try
            {
                var data = servicio.ListarContratosActivos();
                return Json(new { consultaExitosa = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ListarSinContrato()
        {
            try
            {
                var data = servicio.ListarSinContratoActivo();
                return Json(new { consultaExitosa = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerAreas()
        {
            var areas = _areaService.ObtenerAreas()
                .Select(a => new
                {
                    id = a.AreaId,
                    nombre = a.AreaNombre
                })
                .ToList();

            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerCargos()
        {
            var cargos = _cargoService.ObtenerCargos()
                .Select(c => new
                {
                    id = c.CargoId,
                    nombre = c.CargoNombre
                })
                .ToList();

            return Json(cargos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerPensiones()
        {
            var pensiones = _pensionService.ObtenerSistemasPensiones()
                .Select(p => new
                {
                    id = p.TipoPensionId,
                    nombre = p.Nombre,
                    entidad = p.Entidad
                })
                .ToList();

            return Json(pensiones, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerTiposSalarios()
        {
            var tipos = _tipoSalarioService.ObtenerTiposSalarios()
                .Select(t => new
                {
                    id = t.TipoSalarioId,
                    nombre = t.TipoSalarioNombre
                })
                .ToList();

            return Json(tipos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerTiposJornadas()
        {
            var tipos = _jornadaService.ObtenerTiposJornadas()
                .Select(j => new
                {
                    id = j.TipoJornadaId,
                    nombre = j.TipoJornadaNombre
                })
                .ToList();

            return Json(tipos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CrearContrato(ContratoCrearModel modelo)
        {
            try
            {
                var contrato = new Contrato
                {
                    Trabajador = new Trabajador { TrabajadorId = modelo.TrabajadorId },
                    Cargo = modelo.CargoId.HasValue ? new Cargo { CargoId = modelo.CargoId.Value } : null,
                    Area = modelo.AreaId.HasValue ? new Area { AreaId = modelo.AreaId.Value } : null,
                    TipoPension = modelo.TipoPensionId.HasValue ? new TipoPension { TipoPensionId = modelo.TipoPensionId.Value } : null,
                    TipoSalario = modelo.TipoSalarioId.HasValue ? new TipoSalario { TipoSalarioId = modelo.TipoSalarioId.Value } : null,
                    TipoJornada = modelo.TipoJornadaId.HasValue 
                        ? new capa_dominio.TipoJornada { TipoJornadaId = modelo.TipoJornadaId.Value } 
                        : null,
                    ContratoFechaInicio = modelo.FechaInicio,
                    ContratoFechaFin = modelo.FechaFin,
                    ContratoSalario = modelo.Salario ?? 0,
                    ContratoTarifaHora = modelo.TarifaHora ?? 0,
                    ContratoModoPago = modelo.ModoPago,
                    ContratoDescripcionFunciones = modelo.DescripcionFunciones,
                    ContratoObservaciones = modelo.Observaciones,
                    EstadoiId = 1
                };

                var nuevoId = servicio.CrearContrato(contrato);

                return Json(new { exito = true, contratoId = nuevoId });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
    }

    public class ContratoCrearModel
    {
        public int TrabajadorId { get; set; }
        public int? CargoId { get; set; }
        public int? AreaId { get; set; }
        public int? TipoPensionId { get; set; }
        public int? TipoSalarioId { get; set; }
        public int? TipoJornadaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? Salario { get; set; }
        public decimal? TarifaHora { get; set; }
        public string ModoPago { get; set; }
        public string DescripcionFunciones { get; set; }
        public string Observaciones { get; set; }
    }
}