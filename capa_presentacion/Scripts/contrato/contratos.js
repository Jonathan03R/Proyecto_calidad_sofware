(function () {

    'use strict';

    const $page = $('#page-contratos');
    const URLS = {
        listarActivos: $page.data('url-listar-activos'),
        listarSin: $page.data('url-listar-sin'),
        obtenerAreas: $page.data('url-obtener-areas'),
        obtenerCargos: $page.data('url-obtener-cargos'),
        obtenerPensiones: $page.data('url-obtener-pensiones'),
        obtenerTiposSalarios: $page.data('url-obtener-tipos-salarios'),
        obtenerJornadas: $page.data('url-obtener-jornadas'),
        crearContrato: $page.data('url-crear-contrato')
    };

    const Cache = { activos: [], sin: [] };
    const Lookups = { cargos: null, areas: null };

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
        $.get(URLS.listarSin, resp => {
            if (!resp || !resp.consultaExitosa) {
                $tb.html(emptyRow(3, resp?.mensaje || 'No se pudo obtener la lista'));
                return;
            }
            Cache.sin = resp.data || [];
            aplicarFiltro();
        }).fail(() => $tb.html(emptyRow(3, 'Error de conexión')));
    }

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
            $sel.append(<option value="${x[idKey]}">${(x[textKey] || '').toString()}</option>);
        });
    }
    // ---------- Eventos de tabs / filtro ----------
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
        if (e.key === 'Enter') {
            e.preventDefault();
            aplicarFiltro();
        }
    });

    let t;
    $(document).on('input', '#fc_query', function () {
        clearTimeout(t);
        t = setTimeout(aplicarFiltro, 150);
    });

    $(document).on('reset', '#form-filtros-contratos', () => setTimeout(aplicarFiltro, 0));

    // ---------- Primera carga ----------
    $(function () { cargarActivos(); });

    // (Opcional) expone funciones globales
    window.ContratosUI = { recargarActivos: cargarActivos, recargarSin: cargarSin };
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

    // ---------- Cargar Áreas ----------
    function cargarAreas() {
        const $select = $('#nc_area_id');
        $select.empty().append('<option value="">Seleccione área</option>');

        $.getJSON(URLS.obtenerAreas)
            .done(function (data) {
                (data || []).forEach(function (area) {
                    $select.append(
                        $('<option>', {
                            value: area.id,
                            text: area.nombre
                        })
                    );
                });
            })
            .fail(function () {
                console.error('Error al cargar áreas');
            });
    }
    // ---------- Cargar Cargos ----------
    function cargarCargos() {
        const $select = $('#nc_cargo_id');
        $select.empty().append('<option value="">Seleccione cargo</option>');

        $.getJSON(URLS.obtenerCargos)
            .done(function (data) {
                (data || []).forEach(function (cargo) {
                    $select.append(
                        $('<option>', {
                            value: cargo.id,
                            text: cargo.nombre
                        })
                    );
                });
            })
            .fail(function () {
                console.error('Error al cargar cargos');
            });
    }
    // ---------- Cargar Pensiones ----------
    function cargarPensiones() {
        const $select = $('#nc_tipo_pension_id');
        $select.empty().append('<option value="">Seleccione</option>');

        $.getJSON(URLS.obtenerPensiones)
            .done(function (data) {
                (data || []).forEach(function (p) {
                    const texto = p.entidad
                        ? `${p.nombre} (${p.entidad})`
                        : p.nombre;

                $select.append(
                    $('<option>', {
                        value: p.id,
                        text: texto
                    })
                );
            });
    })
            .fail(function () {
        console.error('Error al cargar pensiones');
    });
}
    // ---------- Cargar Tipos de Salario ----------
    function cargarTiposSalarios() {
    const $select = $('#nc_tipo_salario_id');
    $select.empty().append('<option value="">Seleccione</option>');

    $.getJSON(URLS.obtenerTiposSalarios)
        .done(function (data) {
            (data || []).forEach(function (t) {
                $select.append(
                    $('<option>', {
                        value: t.id,
                        text: t.nombre
                    })
                );
            });
        })
        .fail(function () {
            console.error('Error al cargar tipos de salario');
        });
}
// ---------- Cargar jornadas ----------
function cargarJornadas() {
    const $select = $('#nc_tipo_jornada_id');
    $select.empty().append('<option value="">Seleccione</option>');

    $.getJSON(URLS.obtenerJornadas)
        .done(function (data) {
            (data || []).forEach(function (j) {
                $select.append(
                    $('<option>', {
                        value: j.id,
                        text: j.nombre
                    })
                );
            });

            // 👉 Seleccionar por defecto TIEMPO COMPLETO (ID = 1)
            $select.val("1");
        })
        .fail(function () {
            console.error('Error al cargar tipos de jornada');
        });
}

// ===== Confirmar creación de contrato =====
$(document).on('click', '#nc_confirmar', function () {

    const contrato = {
        TrabajadorId: Number($('#nc_trabajador_id').val()),
        CargoId: $('#nc_cargo_id').val() ? Number($('#nc_cargo_id').val()) : null,
        AreaId: $('#nc_area_id').val() ? Number($('#nc_area_id').val()) : null,
        TipoPensionId: $('#nc_tipo_pension_id').val() ? Number($('#nc_tipo_pension_id').val()) : null,
        TipoSalarioId: $('#nc_tipo_salario_id').val() ? Number($('#nc_tipo_salario_id').val()) : null,
        TipoJornadaId: $('#nc_tipo_jornada_id').val() ? Number($('#nc_tipo_jornada_id').val()) : 1, // default 1
        FechaInicio: $('#nc_fecha_inicio').val(),             // "yyyy-MM-dd"
        FechaFin: $('#nc_fecha_fin').val() || null,
        Salario: $('#nc_remuneracion').val() ? parseFloat($('#nc_remuneracion').val()) : null,
        TarifaHora: $('#nc_tarifa_hora').val() ? parseFloat($('#nc_tarifa_hora').val()) : null,
        ModoPago: $('#nc_modo_pago').val() || null,
        DescripcionFunciones: $('#nc_descripcion_funciones').val() || null,
        Observaciones: $('#nc_observaciones').val() || null
    };

    // Validación básica
    if (!contrato.TrabajadorId) {
        $('#nc_mensaje').text('Falta el trabajador.');
        return;
    }
    if (!contrato.CargoId) {
        $('#nc_mensaje').text('Seleccione un cargo.');
        return;
    }
    if (!contrato.AreaId) {
        $('#nc_mensaje').text('Seleccione un área.');
        return;
    }
    if (!contrato.TipoPensionId) {
        $('#nc_mensaje').text('Seleccione el sistema de pensiones.');
        return;
    }
    if (!contrato.TipoSalarioId) {
        $('#nc_mensaje').text('Seleccione el tipo de salario.');
        return;
    }
    if (!contrato.FechaInicio) {
        $('#nc_mensaje').text('Ingrese la fecha de inicio.');
        return;
    }
    if (!contrato.Salario || isNaN(contrato.Salario)) {
        $('#nc_mensaje').text('Ingrese un salario válido.');
        return;
    }

    $('#nc_mensaje').text('Guardando contrato...');

    $.ajax({
        url: URLS.crearContrato,
        type: 'POST',
        data: JSON.stringify(contrato),
        contentType: 'application/json; charset=utf-8',
        success: function (resp) {
            if (resp && resp.exito) {
                $('#nc_mensaje').text('Contrato creado correctamente.');
                // cerrar modal
                setTimeout(function () {
                    $('#nc_mensaje').text('');
                    // recargar tablas
                    if (window.ContratosUI) {
                        window.ContratosUI.recargarActivos();
                        window.ContratosUI.recargarSin();
                    }
                    // cerrar modal
                    $('[data-modal-close="modal-nuevo-contrato"]').click();
                }, 700);
            } else {
                $('#nc_mensaje').text(resp && resp.mensaje ? resp.mensaje : 'No se pudo crear el contrato.');
            }
        },
        error: function () {
            $('#nc_mensaje').text('Error de conexión al crear el contrato.');
        }
    });
});

$(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
    const id = Number($(this).data('trabid'));
    const item = (Cache.sin || []).find(x => Number(x.TrabajadorId) === id);
    if (!item) return;
    $('#nc_trabajador_id').val(id);
    $('#nc_nombre').val(item.EmpleadoNombre || '');
    $('#nc_dni').val(item.Documento || '');
    // Limpiar campos del contrato
    $('#nc_cargo_id').val('');
    $('#nc_area_id').val('');
    $('#nc_tipo_pension_id').val('');
    $('#nc_tipo_salario_id').val('');
    $('#nc_tipo_jornada_id').val('');
    $('#nc_modo_pago').val('');
    $('#nc_fecha_inicio').val('');
    $('#nc_fecha_fin').val('');
    $('#nc_remuneracion').val('');
    $('#nc_tarifa_hora').val('');
    $('#nc_descripcion_funciones').val('');
    $('#nc_observaciones').val('');
    $('#nc_mensaje').text('');

    // 👉 Cargar áreas dinámicamente
    cargarAreas();
    cargarCargos();
    cargarPensiones();
    cargarTiposSalarios();
    cargarJornadas();

    // Abrir modal
    openModal('modal-nuevo-contrato');
    setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
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

}) (jQuery);