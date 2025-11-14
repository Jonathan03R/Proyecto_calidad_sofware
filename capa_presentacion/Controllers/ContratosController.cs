using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_aplicacion.sevicios.Tipos_salarios;
using capa_dominio.dto;
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
                var resultado = data.Select(x => new
                {
                    // Para tabla
                    ContratoId = x.ContratoId,
                    TrabajadorId = x.TrabajadorId,
                    EmpleadoNombre = x.EmpleadoNombre,
                    Documento = x.Documento,
                    CargoNombre = x.CargoNombre,
                    EstadoContratoNombre = x.EstadoContratoNombre,
                    FechaInicio = x.FechaInicio == DateTime.MinValue ? "" : x.FechaInicio.ToString("yyyy-MM-dd"),
                    FechaFin = x.FechaFin.HasValue ? x.FechaFin.Value.ToString("yyyy-MM-dd") : "",

                    // Para modal
                    CargoId = x.CargoId,
                    TipoSalarioId = x.TipoSalarioId,
                    Salario = x.Salario,
                    ModoPago = x.ModoPago,
                    Observaciones = x.Observaciones,

                    // Otros datos por si luego los usas
                    AreaId = x.AreaId,
                    TipoPensionId = x.TipoPensionId,
                    TipoJornadaId = x.TipoJornadaId,
                    TarifaHora = x.TarifaHora,
                    HorasSemanales = x.HorasSemanales,
                    DescripcionFunciones = x.DescripcionFunciones
                });
                return Json(new { consultaExitosa = true, data = resultado }, JsonRequestBehavior.AllowGet);
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
                var resultado = data.Select(x => new
                {
                    TrabajadorId = x.TrabajadorId,
                    EmpleadoNombre = x.EmpleadoNombre,
                    Documento = x.Documento,
                    EstadoContratoNombre = x.EstadoContratoNombre
                });
                return Json(new { consultaExitosa = true, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { consultaExitosa = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        // ✅ Obtener Areas
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
        // ✅ Obtener Cargos
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

        // ✅ Obtener sistemas de pensiones (AFP / ONP)
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

        // ✅ Obtener tipos de salario
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

        // ✅ Obtener tipos de jornadas
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

        // ✅ CRear Contrato
        [HttpPost]
        public JsonResult CrearContrato(ContratoDTO contrato)
        {
            try
            {
                var nuevoId = servicio.CrearContrato(contrato);

                return Json(new
                {
                    exito = true,
                    contratoId = nuevoId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message  
                });
            }
        }

        // ✅ Actualizar Contrato
        [HttpPost]
        public JsonResult ActualizarContrato(ContratoDTO contrato, string motivo)
        {
            try
            {
                if (!contrato.ContratoId.HasValue || contrato.ContratoId.Value <= 0)
                    throw new Exception("El contrato a actualizar no es válido.");

                if (string.IsNullOrWhiteSpace(motivo))
                    throw new Exception("Debes indicar el motivo de la actualización.");

                var usuario = User?.Identity != null && User.Identity.IsAuthenticated
                    ? User.Identity.Name
                    : Environment.UserName; 

                servicio.ActualizarContrato(contrato.ContratoId.Value, usuario, motivo, contrato);

                return Json(new
                {
                    exito = true,
                    mensaje = "Contrato actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult ResumenContratos()
        {
            try
            {
                var resumen = servicio.ObtenerResumen();
                return Json(new
                {
                    exito = true,
                    data = resumen
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
