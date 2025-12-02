var HistorialNomina = (function () {

    let page = 1;
    let pageSize = 10;

    function init() {
        cargarPeriodos();
        eventos();
        cargarTabla();
    }

    function eventos() {
        $("#btnFiltrar").on("click", function () {
            page = 1;
            cargarTabla();
        });

        $("#filtroBuscar").on("keyup", function (e) {
            if (e.key === "Enter") {
                page = 1;
                cargarTabla();
            }
        });
    }

    function cargarPeriodos() {
        $.getJSON("/Nomina/ListarPeriodosHistorial", res => {
            if (!res.ok) return;

            const select = $("#filtroPeriodo");
            select.empty();
            select.append(`<option value="">Todos</option>`);

            res.data.forEach(x => {
                select.append(`
                    <option value="${x.PeriodoId}">
                        ${x.PeriodoNombre}
                    </option>
                `);
            });
        });
    }

    function cargarTabla() {

        $.getJSON("/Nomina/ListarHistorialPaginado", {
            page: page,
            pageSize: pageSize,
            periodoId: $("#filtroPeriodo").val(),
            estado: $("#filtroEstado").val(),
            buscar: $("#filtroBuscar").val()
        })
            .done(res => {
                if (!res.ok) {
                    console.warn(res.msg);
                    return;
                }

                const r = res.data;
                renderTabla(r.Items);
                renderPaginacion(r.Page, r.TotalPages);
            });
    }

    function renderTabla(items) {
        const tbody = $("#tblHistorialBody");
        tbody.empty();

        if (!items || items.length === 0) {
            tbody.append(`<tr><td colspan="7">Sin datos</td></tr>`);
            return;
        }

        items.forEach(x => {
            tbody.append(`
                <tr>
                    <td>${x.PeriodoNombre}</td>
                    <td>${x.NominaEstado}</td>
                    <td>${x.NominaTotalEmpleados}</td>
                    <td>${x.NominaTotalBruto}</td>
                    <td>${x.NominaTotalDescuentos}</td>
                    <td>${x.NominaTotalNeto}</td>
                    <td>${formatearFecha(x.NominaFechaProcesamiento)}</td>
                </tr>
            `);
        });
    }

    function renderPaginacion(current, total) {
        const cont = $("#paginacion");
        cont.empty();

        if (!total || total <= 1) return;

        let html = `<div class="pagination">`;

        html += `<button class='btn-page' data-p='${current - 1}' ${current === 1 ? "disabled" : ""}>‹</button>`;

        for (let i = 1; i <= total; i++) {
            html += `<button class='btn-page ${i === current ? "active" : ""}' data-p='${i}'>${i}</button>`;
        }

        html += `<button class='btn-page' data-p='${current + 1}' ${current === total ? "disabled" : ""}>›</button>`;

        html += `</div>`;
        cont.html(html);

        cont.find(".btn-page").click(function () {
            const p = parseInt($(this).data("p"));
            if (p >= 1 && p <= total) {
                page = p;
                cargarTabla();
            }
        });
    }

    function formatearFecha(fechaIso) {
        if (!fechaIso) return "";
        const d = new Date(fechaIso);
        if (isNaN(d)) return fechaIso;
        return d.toLocaleDateString("es-PE");
    }

    return { init };

})();

$(function () {
    HistorialNomina.init();
});
