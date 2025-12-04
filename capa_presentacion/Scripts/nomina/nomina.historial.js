// Lista original que viene del backend
let historialOriginal = [];

$(function () {
    cargarHistorial();

    // Eventos de filtros
    $('#txtBuscar').on('input', aplicarFiltros);
    $('#filtroFechaEjecucion').on('change', aplicarFiltros);
    $('#filtroEstado').on('change', aplicarFiltros);

    $('#btnLimpiarFiltros').on('click', function () {
        $('#txtBuscar').val('');
        $('#filtroFechaEjecucion').val('');
        $('#filtroEstado').val('');
        aplicarFiltros();   // vuelve a mostrar toda la tabla
    });
});

function cargarHistorial() {
    $.ajax({
        url: '/Nomina/ListarResumenNominas',
        method: 'GET',
        success: function (resp) {
            if (!resp.consultaExitosa) {
                alert(resp.mensaje || 'Error al cargar el historial.');
                return;
            }

            historialOriginal = resp.data || [];
            aplicarFiltros();   // primera carga sin filtros
        },
        error: function () {
            alert('Error de comunicación con el servidor.');
        }
    });
}

// Aplica Buscar + Fecha + Estado sobre la lista original
function aplicarFiltros() {
    const textoBuscar = ($('#txtBuscar').val() || '').trim().toLowerCase();
    const estadoSeleccionado = $('#filtroEstado').val();
    const fechaFiltro = $('#filtroFechaEjecucion').val(); // "2025-11-26"

    const filtrados = historialOriginal.filter(item => {
        // --- filtro por texto (período) ---
        const periodoCrudo = (item.PeriodoNombre || '').toLowerCase();              // "2025-10"
        const periodoBonito = formatearPeriodo(item.PeriodoNombre).toLowerCase();   // "octubre 2025"

        let coincideBusqueda = true;
        if (textoBuscar) {
            coincideBusqueda =
                periodoCrudo.includes(textoBuscar) ||
                periodoBonito.includes(textoBuscar);
        }
        if (!coincideBusqueda) return false;

        // --- filtro por estado ---
        if (estadoSeleccionado && item.NominaEstado !== estadoSeleccionado) {
            return false;
        }

        // --- filtro por fecha de ejecución ---
        if (fechaFiltro) {
            const fechaItem = parseNetDate(item.NominaFechaProcesamiento); // Date
            if (!fechaItem) return false;

            const yyyyMmDdItem = fechaItem.toISOString().substring(0, 10);
            if (yyyyMmDdItem !== fechaFiltro) {
                return false;
            }
        }

        return true;
    });

    renderTabla(filtrados);
}

// Renderiza la tabla con una lista (filtrada o no)
function renderTabla(lista) {
    const tbody = $("#tblHistorial tbody");
    const template = $("#row-template");

    // limpiar filas anteriores (menos la plantilla)
    tbody.find("tr:not(#row-template)").remove();

    if (!lista.length) {
        const tr = $("<tr></tr>");
        tr.append('<td colspan="6" class="t-center">Sin datos</td>');
        tbody.append(tr);
        $("#hist-resumen").text("Mostrando 0 registros");
        return;
    }

    lista.forEach(item => {
        const row = template.clone().removeAttr("id").show();

        // Período: "Octubre 2025"
        row.find(".td-periodo").text(formatearPeriodo(item.PeriodoNombre));

        // Fecha ejecución bonita
        row.find(".td-fecha").text(formatNetDate(item.NominaFechaProcesamiento));
        row.find(".td-empleados").text(item.NominaTotalEmpleados);
        row.find(".td-estado").text(item.NominaEstado);
        row.find(".td-total").text(item.NominaTotalNeto.toFixed(2));

        // Por si luego quieres usar data-*
        row.attr("data-periodo", item.PeriodoNombre);
        row.attr("data-estado", item.NominaEstado);

        tbody.append(row);
    });

    $("#hist-resumen").text(`Mostrando ${lista.length} registro(s)`);
}

// Convierte "/Date(1764149837623)/" a objeto Date
function parseNetDate(netDateString) {
    if (!netDateString) return null;
    const match = /\/Date\((\d+)\)\//.exec(netDateString);
    if (!match) return null;
    const millis = parseInt(match[1], 10);
    const d = new Date(millis);
    return isNaN(d.getTime()) ? null : d;
}

// Convierte "/Date(1764149837623)/" a "30/11/2025 09:15"
function formatNetDate(netDateString) {
    const d = parseNetDate(netDateString);
    if (!d) return netDateString || '';

    return d.toLocaleString('es-PE', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// "2025-11" -> "Noviembre 2025"
function formatearPeriodo(periodo) {
    if (!periodo) return '';
    const partes = periodo.split("-");
    if (partes.length !== 2) return periodo;

    const año = Number.parseInt(partes[0], 10);
    const mes = Number.parseInt(partes[1], 10) - 1;

    const fecha = new Date(año, mes, 1);

    return fecha.toLocaleDateString("es-PE", {
        month: "long",
        year: "numeric"
    });
}
