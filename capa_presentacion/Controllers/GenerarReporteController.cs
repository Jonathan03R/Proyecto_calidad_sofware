using capa_aplicacion.Servicios;
using capa_aplicacion.sevicios;
using capa_dominio;
using capa_dominio.dto;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace capa_presentacion.Controllers
{
    public class GenerarReporteController : Controller
    {
        private readonly ReporteService reporteService;
        private readonly PeriodoService periodoService;

        public GenerarReporteController()
        {
            reporteService = new ReporteService();
            periodoService = new PeriodoService();
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
                listaPeriodos = periodoService.ListarProcesados();
                foreach (var p in listaPeriodos)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[PERIODO] id={p.PeriodoId}, nombre={p.PeriodoNombre}, inicio={p.PeriodoFechaInicio}, fin={p.PeriodoFechaFin}, estado={p.EstadoId} - {p.EstadoNombre}"
                    );
                }
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

        [HttpGet]
        public ActionResult GenerarPDF(int periodoId, int? cargoId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[PDF] Generando reporte - PeriodoId: {periodoId}, CargoId: {cargoId}");

                // Obtener datos
                var datos = reporteService.ConsultarNominaPorPeriodo(periodoId, cargoId);

                if (datos == null || !datos.Any())
                {
                    System.Diagnostics.Debug.WriteLine("[PDF] No se encontraron datos");
                    return Json(new
                    {
                        consultaExitosa = false,
                        mensaje = "No hay datos para exportar en este período"
                    }, JsonRequestBehavior.AllowGet);
                }

                System.Diagnostics.Debug.WriteLine($"[PDF] Se encontraron {datos.Count} registros");

                // Obtener información del período
                var periodos = periodoService.ListarProcesados();
                var periodo = periodos.FirstOrDefault(p => p.PeriodoId == periodoId);
                var nombrePeriodo = periodo?.PeriodoNombre ?? $"Periodo_{periodoId}";

                System.Diagnostics.Debug.WriteLine($"[PDF] Período: {nombrePeriodo}");

                using (var stream = new MemoryStream())
                {
                    // Crear documento en orientación horizontal
                    var document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                    var writer = PdfWriter.GetInstance(document, stream);

                    document.Open();

                    // === TÍTULO ===
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var title = new Paragraph("REPORTE DE NÓMINA\n\n", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);

                    // === INFORMACIÓN DEL PERÍODO ===
                    var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    var info = new Paragraph(string.Format("Período: {0}\nGenerado: {1:dd/MM/yyyy HH:mm}\nTotal de registros: {2}\n\n",
                        nombrePeriodo, DateTime.Now, datos.Count), infoFont);
                    document.Add(info);

                    // === TABLA RESUMEN ===
                    var table = new PdfPTable(8) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 10, 15, 15, 12, 12, 12, 12, 12 });

                    // Encabezados
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
                    var headersArray = new[] { "Código", "Nombres", "Apellidos", "Tipo ID", "Número ID",
                                         "Sueldo Básico", "Total Desc.", "Neto a Pagar" };

                    foreach (var header in headersArray)
                    {
                        var cell = new PdfPCell(new Phrase(header, headerFont))
                        {
                            BackgroundColor = new BaseColor(49, 130, 206),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5
                        };
                        table.AddCell(cell);
                    }

                    // Datos
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
                    decimal totalSueldos = 0;
                    decimal totalDescuentos = 0;
                    decimal totalNeto = 0;

                    foreach (var item in datos)
                    {
                        table.AddCell(new PdfPCell(new Phrase(item.CodigoTrabajador ?? "", dataFont)) { Padding = 3 });
                        table.AddCell(new PdfPCell(new Phrase(item.Nombres ?? "", dataFont)) { Padding = 3 });
                        table.AddCell(new PdfPCell(new Phrase(item.Apellidos ?? "", dataFont)) { Padding = 3 });
                        table.AddCell(new PdfPCell(new Phrase(item.TipoDeIdentificacion ?? "", dataFont)) { Padding = 3 });
                        table.AddCell(new PdfPCell(new Phrase(item.NumeroIdentificacion ?? "", dataFont)) { Padding = 3 });

                        table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", item.SueldoBasico), dataFont))
                        { Padding = 3, HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", item.TotalDescuentos), dataFont))
                        { Padding = 3, HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", item.NetoPagar), dataFont))
                        { Padding = 3, HorizontalAlignment = Element.ALIGN_RIGHT });

                        totalSueldos += item.SueldoBasico;
                        totalDescuentos += item.TotalDescuentos;
                        totalNeto += item.NetoPagar;
                    }

                    // Fila de totales
                    var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                    var totalCell = new PdfPCell(new Phrase("TOTALES", totalFont))
                    {
                        Colspan = 5,
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Padding = 5
                    };
                    table.AddCell(totalCell);
                    table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", totalSueldos), totalFont))
                    { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", totalDescuentos), totalFont))
                    { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase(string.Format("S/ {0:N2}", totalNeto), totalFont))
                    { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });

                    document.Add(table);

                    // === NOTA AL PIE ===
                    var noteFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8);
                    var note = new Paragraph("\n\nNota: Este es un reporte resumido. " +
                        "Para ver todos los detalles, descargue el archivo Excel.", noteFont);
                    document.Add(note);

                    document.Close();

                    var fileName = string.Format("Reporte_Nomina_{0}_{1:yyyyMMdd_HHmmss}.pdf",
                        nombrePeriodo.Replace(" ", "_"), DateTime.Now);

                    System.Diagnostics.Debug.WriteLine($"[PDF] Archivo generado: {fileName}");

                    return File(stream.ToArray(), "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    consultaExitosa = false,
                    mensaje = "Error al generar PDF: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
