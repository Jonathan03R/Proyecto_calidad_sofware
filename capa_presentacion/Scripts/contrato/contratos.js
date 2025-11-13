//// ============================
//// CONTRATOS.JS - 💙
//// ============================
//(function () {
//    'use strict';

//    // ---------- Config desde la vista ----------
//    const $page = $('#page-contratos');
//    const URLS = {
//        listarActivos: $page.data('url-listar-activos'),
//        listarSin: $page.data('url-listar-sin'),
//    };

//    // ---------- Caché ----------
//    const Cache = { activos: [], sin: [] };

//    // ---------- Utils ----------
//    const norm = s => (s ?? '').toString()
//        .normalize('NFD').replace(/[\u0300-\u036f]/g, '')
//        .toLowerCase().trim();

//    const coincide = (it, qn) => !qn ||
//        norm(it.EmpleadoNombre).includes(qn) || norm(it.Documento).includes(qn);

//    const fmtFecha = v => {
//        if (!v) return '';
//        const d = new Date(v);
//        return isNaN(d) ? String(v) : d.toLocaleDateString('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
//    };
//    const esc = t => {
//        if (t == null) return '';
//        const m = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
//        return String(t).replace(/[&<>"']/g, s => m[s]);
//    };
//    const emptyRow = (c, t) => `<tr><td colspan="${c}"><div class="empty-state"><div class="empty-state-icon">📋</div><p>${esc(t)}</p></div></td></tr>`;
//    const loadingRow = (c) => `<tr><td colspan="${c}"><div class="loading"><div class="spinner"></div><p>Cargando...</p></div></td></tr>`;

//    const tabActiva = () => $('.tab-btn.is-active').data('tab') || 'activos';

//    // ---------- Render ----------
//    function renderActivos(items) {
//        const $tb = $('#tbody-contratos');
//        if (!items.length) {
//            $tb.html(emptyRow(6, 'Sin contratos activos'));
//            $('#txt-total').text('0');
//            return;
//        }
//        const rows = items.map(it => `
//      <tr>
//        <td><p class="empleado-nombre">${esc(it.EmpleadoNombre)}</p></td>
//        <td>${esc(it.Documento)}</td>
//        <td>${esc(it.CargoNombre || '')}</td>
//        <td>${esc(it.EstadoContratoNombre || '')}</td>
//        <td>${fmtFecha(it.FechaInicio) || '-'}</td>
//        <td>${fmtFecha(it.FechaFin) || '-'}</td>
//      </tr>`).join('');
//        $tb.html(rows);
//        $('#txt-total').text(`${items.length} registro(s)`);
//    }

//    function renderSin(items) {
//        const $tb = $('#tbody-sin-contrato');
//        if (!items.length) {
//            $tb.html(emptyRow(3, 'No hay empleados sin contrato'));
//            $('#txt-total-sin').text('0');
//            return;
//        }
//        const rows = items.map(it => `
//      <tr>
//        <td>${esc(it.EmpleadoNombre)}</td>
//        <td>${esc(it.Documento)}</td>
//        <td><button class="btn btn-primary" data-trabid="${it.TrabajadorId}">Nuevo Contrato</button></td>
//      </tr>`).join('');
//        $tb.html(rows);
//        $('#txt-total-sin').text(`${items.length} sin contrato`);
//    }

//    // ---------- Carga desde servidor (guarda en caché) ----------
//    function cargarActivos() {
//        const $tb = $('#tbody-contratos');
//        $tb.html(loadingRow(6));
//        $('#txt-total').text(''); $('#paginador').empty();

//        $.get(URLS.listarActivos, resp => {
//            if (!resp || !resp.consultaExitosa) {
//                $tb.html(emptyRow(6, resp?.mensaje || 'No se pudieron obtener los contratos activos'));
//                return;
//            }
//            Cache.activos = resp.data || [];
//            aplicarFiltro();
//        }).fail(() => $tb.html(emptyRow(6, 'Error de conexión')));
//    }

//    function cargarSin() {
//        const $tb = $('#tbody-sin-contrato');
//        $tb.html(loadingRow(3));
//        $('#txt-total-sin').text('');

//        $.get(URLS.listarSin, resp => {
//            if (!resp || !resp.consultaExitosa) {
//                $tb.html(emptyRow(3, resp?.mensaje || 'No se pudo obtener la lista'));
//                return;
//            }
//            Cache.sin = resp.data || [];
//            aplicarFiltro();
//        }).fail(() => $tb.html(emptyRow(3, 'Error de conexión')));
//    }

//    // ---------- Filtro ----------
//    function aplicarFiltro() {
//        const qn = norm($('#fc_query').val());
//        if (tabActiva() === 'activos') {
//            renderActivos((Cache.activos || []).filter(x => coincide(x, qn)));
//        } else {
//            renderSin((Cache.sin || []).filter(x => coincide(x, qn)));
//        }
//    }

//    // ---------- Eventos ----------
//    $(document).on('click', '.tab-btn', function () {
//        const tab = $(this).data('tab');
//        $('.tab-btn').removeClass('is-active');
//        $(this).addClass('is-active');
//        $('.tab-panel').removeClass('is-active');
//        $('#tab-' + tab).addClass('is-active');
//        tab === 'activos' ? cargarActivos() : cargarSin();
//    });

//    $(document).on('click', '#fc_filtrar', aplicarFiltro);
//    $(document).on('keydown', '#fc_query', e => { if (e.key === 'Enter') { e.preventDefault(); aplicarFiltro(); } });

//    let t;
//    $(document).on('input', '#fc_query', function () {
//        clearTimeout(t); t = setTimeout(aplicarFiltro, 150);
//    });

//    $(document).on('reset', '#form-filtros-contratos', () => setTimeout(aplicarFiltro, 0));

//    // ---------- Primera carga ----------
//    $(function () { cargarActivos(); });

//    // (Opcional) expone funciones por si luego quieres recargar desde otro script
//    window.ContratosUI = { recargarActivos: cargarActivos, recargarSin: cargarSin };

//    // ===== Modal helpers =====
//    function openModal(id) {
//        const $m = $('#' + id);
//        $m.attr('aria-hidden', 'false').addClass('is-open');
//        $('body').addClass('modal-open'); // evita scroll del body (si tienes estilos)
//    }
//    function closeModal(id) {
//        const $m = $('#' + id);
//        $m.attr('aria-hidden', 'true').removeClass('is-open');
//        $('body').removeClass('modal-open');
//    }

//    // Cerrar por click en [data-modal-close] o backdrop
//    $(document).on('click', '[data-modal-close]', function () {
//        closeModal($(this).data('modal-close'));
//    });
//    $(document).on('click', '#modal-nuevo-contrato .modal-backdrop', function () {
//        closeModal('modal-nuevo-contrato');
//    });
//    // Cerrar con ESC
//    $(document).on('keydown', function (e) {
//        if (e.key === 'Escape') closeModal('modal-nuevo-contrato');
//    });

//    // ===== Abrir "Nuevo Contrato" desde la lista de SIN contrato =====
//    // Nota: tus filas ya tienen <button class="btn btn-primary" data-trabid="...">
//    $(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
//        const id = Number($(this).data('trabid'));

//        // Busca al trabajador en cache (Cache.sin lo llenas cuando cargas "sin contrato")
//        const item = (Cache.sin || []).find(x => Number(x.TrabajadorId) === id);
//        if (!item) return;

//        // Prellenar campos
//        $('#nc_trabajador_id').val(id);
//        $('#nc_nombre').val(item.EmpleadoNombre || '');
//        $('#nc_dni').val(item.Documento || '');

//        // (Opcional) limpia otros campos del modal
//        $('#nc_cargo').val('');
//        $('#nc_domicilio').val('');
//        $('#nc_fecha_inicio').val('');
//        $('#nc_fecha_fin').val('');
//        $('#nc_hora_inicio').val('');
//        $('#nc_hora_fin').val('');
//        $('#nc_remuneracion').val('');
//        $('#nc_mensaje').text('');

//        // Abrir modal y enfocar
//        openModal('modal-nuevo-contrato');
//        setTimeout(() => $('#nc_cargo').trigger('focus'), 50);
//    });




//    (function ($) {
//        'use strict';

//        // ... (tu código actual)

//        // === URLs desde la vista ===
//        const $page = $('#page-contratos');
//        const URLS = {
//            listarActivos: $page.data('url-listar-activos'),
//            listarSin: $page.data('url-listar-sin'),
//            cargos: $page.data('url-cargos'),
//            areas: $page.data('url-areas')
//        };

//        // Lookups cache
//        const Lookups = { cargos: null, areas: null };

//        function fillSelect($sel, arr, idKey, textKey) {
//            $sel.empty().append('<option value="">-- Seleccione --</option>');
//            (arr || []).forEach(x => {
//                $sel.append(`<option value="${x[idKey]}">${(x[textKey] || '').toString()}</option>`);
//            });
//        }

//        function cargarCargos() {
//            if (Lookups.cargos) return $.Deferred().resolve(Lookups.cargos).promise();
//            return $.get(URLS.cargos, resp => {
//                if (!resp || !resp.consultaExitosa) throw new Error(resp?.mensaje || 'Error cargos');
//                Lookups.cargos = resp.data || [];
//            });
//        }

//        function cargarAreas() {
//            if (Lookups.areas) return $.Deferred().resolve(Lookups.areas).promise();
//            return $.get(URLS.areas, resp => {
//                if (!resp || !resp.consultaExitosa) throw new Error(resp?.mensaje || 'Error áreas');
//                Lookups.areas = resp.data || [];
//            });
//        }

//        function poblarCombosContrato() {
//            const $cargo = $('#nc_cargo_id');
//            const $area = $('#nc_area_id');
//            fillSelect($cargo, Lookups.cargos, 'CargoId', 'CargoNombre');
//            fillSelect($area, Lookups.areas, 'AreaId', 'AreaNombre');
//        }

//        // Al abrir modal desde “Sin contrato”
//        $(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
//            const id = Number($(this).data('trabid'));

//            // Prellenar nombre/dni como ya lo hacías…
//            const item = (Cache.sin || []).find(x => Number(x.TrabajadorId) === id);
//            if (!item) return;

//            $('#nc_trabajador_id').val(id);
//            $('#nc_nombre').val(item.EmpleadoNombre || '');
//            $('#nc_dni').val(item.Documento || '');

//            // Limpia selección actual
//            $('#nc_cargo_id').val('');
//            $('#nc_area_id').val('');
//            $('#nc_domicilio').val('');
//            $('#nc_fecha_inicio, #nc_fecha_fin, #nc_hora_inicio, #nc_hora_fin, #nc_remuneracion').val('');
//            $('#nc_mensaje').text('');

//            // Cargar lookups (si no están) y poblar selects
//            $.when(cargarCargos(), cargarAreas())
//                .done(() => {
//                    poblarCombosContrato();
//                    openModal('modal-nuevo-contrato');
//                    setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
//                })
//                .fail(err => {
//                    $('#nc_mensaje').text((err && err.message) || 'No se pudieron cargar Cargos/Áreas.');
//                    openModal('modal-nuevo-contrato');
//                });
//        });

//        // ... (tu código de modal open/close y resto)

//    })(jQuery);

//})();

// ============================
// CONTRATOS.JS - 💙
// ============================
(function ($) {
    'use strict';

    // ---------- Config desde la vista ----------
    const $page = $('#page-contratos');
    const URLS = {
        listarActivos: $page.data('url-listar-activos'),
        listarSin: $page.data('url-listar-sin'),
        cargos: $page.data('url-cargos'),
        areas: $page.data('url-areas')
    };

    // ---------- Caché ----------
    const Cache = { activos: [], sin: [] };
    const Lookups = { cargos: null, areas: null };

    // ---------- Utils ----------
    const norm = s => (s ?? '').toString()
        .normalize('NFD').replace(/[\u0300-\u036f]/g, '')
        .toLowerCase().trim();

    const coincide = (it, qn) =>
        !qn ||
        norm(it.EmpleadoNombre).includes(qn) ||
        norm(it.Documento).includes(qn);

    const fmtFecha = v => {
        if (!v) return '';
        const d = new Date(v);
        return isNaN(d)
            ? String(v)
            : d.toLocaleDateString('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
    };

    const esc = t => {
        if (t == null) return '';
        const m = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return String(t).replace(/[&<>"']/g, s => m[s]);
    };

    const emptyRow = (c, t) =>
        `<tr><td colspan="${c}"><div class="empty-state"><div class="empty-state-icon">📋</div><p>${esc(t)}</p></div></td></tr>`;

    const loadingRow = c =>
        `<tr><td colspan="${c}"><div class="loading"><div class="spinner"></div><p>Cargando...</p></div></td></tr>`;

    const tabActiva = () => $('.tab-btn.is-active').data('tab') || 'activos';

    // ---------- Render tablas ----------
    function renderActivos(items) {
        const $tb = $('#tbody-contratos');
        if (!items.length) {
            $tb.html(emptyRow(6, 'Sin contratos activos'));
            $('#txt-total').text('0');
            return;
        }

        const rows = items.map(it => `
          <tr>
            <td><p class="empleado-nombre">${esc(it.EmpleadoNombre)}</p></td>
            <td>${esc(it.Documento)}</td>
            <td>${esc(it.CargoNombre || '')}</td>
            <td>${esc(it.EstadoContratoNombre || '')}</td>
            <td>${fmtFecha(it.FechaInicio) || '-'}</td>
            <td>${fmtFecha(it.FechaFin) || '-'}</td>
          </tr>`).join('');

        $tb.html(rows);
        $('#txt-total').text(`${items.length} registro(s)`);
    }

    function renderSin(items) {
        const $tb = $('#tbody-sin-contrato');
        if (!items.length) {
            $tb.html(emptyRow(3, 'No hay empleados sin contrato'));
            $('#txt-total-sin').text('0');
            return;
        }

        const rows = items.map(it => `
          <tr>
            <td>${esc(it.EmpleadoNombre)}</td>
            <td>${esc(it.Documento)}</td>
            <td><button class="btn btn-primary" data-trabid="${it.TrabajadorId}">Nuevo Contrato</button></td>
          </tr>`).join('');

        $tb.html(rows);
        $('#txt-total-sin').text(`${items.length} sin contrato`);
    }

    // ---------- Carga desde servidor ----------
    function cargarActivos() {
        const $tb = $('#tbody-contratos');
        $tb.html(loadingRow(6));
        $('#txt-total').text('');
        $('#paginador').empty();

        $.get(URLS.listarActivos, resp => {
            if (!resp || !resp.consultaExitosa) {
                $tb.html(emptyRow(6, resp?.mensaje || 'No se pudieron obtener los contratos activos'));
                return;
            }
            Cache.activos = resp.data || [];
            aplicarFiltro();
        }).fail(() => $tb.html(emptyRow(6, 'Error de conexión')));
    }

    function cargarSin() {
        const $tb = $('#tbody-sin-contrato');
        $tb.html(loadingRow(3));
        $('#txt-total-sin').text('');

        $.get(URLS.listarSin, resp => {
            if (!resp || !resp.consultaExitosa) {
                $tb.html(emptyRow(3, resp?.mensaje || 'No se pudo obtener la lista'));
                return;
            }
            Cache.sin = resp.data || [];
            aplicarFiltro();
        }).fail(() => $tb.html(emptyRow(3, 'Error de conexión')));
    }

    // ---------- Filtro ----------
    function aplicarFiltro() {
        const qn = norm($('#fc_query').val());
        if (tabActiva() === 'activos') {
            renderActivos((Cache.activos || []).filter(x => coincide(x, qn)));
        } else {
            renderSin((Cache.sin || []).filter(x => coincide(x, qn)));
        }
    }

    // ---------- Lookups: cargos y áreas ----------
    function fillSelect($sel, arr, idKey, textKey) {
        $sel.empty().append('<option value="">-- Seleccione --</option>');
        (arr || []).forEach(x => {
            $sel.append(`<option value="${x[idKey]}">${(x[textKey] || '').toString()}</option>`);
        });
    }

    function cargarCargos() {
        if (Lookups.cargos) return $.Deferred().resolve(Lookups.cargos).promise();
        return $.get(URLS.cargos, resp => {
            if (!resp || !resp.consultaExitosa) {
                throw new Error(resp?.mensaje || 'Error al cargar cargos');
            }
            Lookups.cargos = resp.data || [];
        });
    }

    function cargarAreas() {
        if (Lookups.areas) return $.Deferred().resolve(Lookups.areas).promise();
        return $.get(URLS.areas, resp => {
            if (!resp || !resp.consultaExitosa) {
                throw new Error(resp?.mensaje || 'Error al cargar áreas');
            }
            Lookups.areas = resp.data || [];
        });
    }

    function poblarCombosContrato() {
        fillSelect($('#nc_cargo_id'), Lookups.cargos, 'CargoId', 'CargoNombre');
        fillSelect($('#nc_area_id'), Lookups.areas, 'AreaId', 'AreaNombre');
    }

    // ---------- Modal helpers ----------
    function openModal(id) {
        const $m = $('#' + id);
        $m.attr('aria-hidden', 'false').addClass('is-open');
        $('body').addClass('modal-open');
    }

    function closeModal(id) {
        const $m = $('#' + id);
        $m.attr('aria-hidden', 'true').removeClass('is-open');
        $('body').removeClass('modal-open');
    }

    $(document).on('click', '[data-modal-close]', function () {
        closeModal($(this).data('modal-close'));
    });

    $(document).on('click', '#modal-nuevo-contrato .modal-backdrop', function () {
        closeModal('modal-nuevo-contrato');
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Escape') closeModal('modal-nuevo-contrato');
    });

    // ---------- Abrir modal desde "Sin contratos" ----------
    $(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
        const id = Number($(this).data('trabid'));
        const item = (Cache.sin || []).find(x => Number(x.TrabajadorId) === id);
        if (!item) return;

        // Prellenar
        $('#nc_trabajador_id').val(id);
        $('#nc_nombre').val(item.EmpleadoNombre || '');
        $('#nc_dni').val(item.Documento || '');

        // Limpiar resto
        $('#nc_cargo_id').val('');
        $('#nc_area_id').val('');
        $('#nc_domicilio').val('');
        $('#nc_fecha_inicio, #nc_fecha_fin, #nc_hora_inicio, #nc_hora_fin, #nc_remuneracion').val('');
        $('#nc_mensaje').text('');

        // Cargar combos y abrir
        $.when(cargarCargos(), cargarAreas())
            .done(() => {
                poblarCombosContrato();
                openModal('modal-nuevo-contrato');
                setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
            })
            .fail(err => {
                $('#nc_mensaje').text((err && err.message) || 'No se pudieron cargar Cargos/Áreas.');
                openModal('modal-nuevo-contrato');
            });
    });

    // ---------- Eventos de tabs y filtro ----------
    $(document).on('click', '.tab-btn', function () {
        const tab = $(this).data('tab');
        $('.tab-btn').removeClass('is-active');
        $(this).addClass('is-active');
        $('.tab-panel').removeClass('is-active');
        $('#tab-' + tab).addClass('is-active');
        tab === 'activos' ? cargarActivos() : cargarSin();
    });

    $(document).on('click', '#fc_filtrar', aplicarFiltro);

    $(document).on('keydown', '#fc_query', e => {
        if (e.key === 'Enter') { e.preventDefault(); aplicarFiltro(); }
    });

    let t;
    $(document).on('input', '#fc_query', function () {
        clearTimeout(t);
        t = setTimeout(aplicarFiltro, 150);
    });

    $(document).on('reset', '#form-filtros-contratos', () =>
        setTimeout(aplicarFiltro, 0)
    );

    // ---------- Primera carga ----------
    $(function () { cargarActivos(); });

    // Por si quieres llamar desde otro script
    window.ContratosUI = {
        recargarActivos: cargarActivos,
        recargarSin: cargarSin
    };

})(jQuery);

