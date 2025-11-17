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
        crearContrato: $page.data('url-crear-contrato'),
        actualizarContrato: $page.data('url-actualizar-contrato'),
        resumenContratos: $page.data('url-resumen-contratos')
    };

    const Cache = { activos: [], sin: [] };

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
            $tb.html(emptyRow(7, 'Sin contratos activos'));
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
                <td>
                    <button
                        type="button"
                        class="btn btn-sm btn-secondary btn-editar-contrato"
                        data-contratoid="${it.ContratoId}"
                        data-trabid="${it.TrabajadorId}">
                        Editar
                    </button>
                </td>
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
                <td>
                    <button class="btn btn-primary" data-trabid="${it.TrabajadorId}">
                        Nuevo Contrato
                    </button>
                </td>
            </tr>`).join('');

        $tb.html(rows);
        $('#txt-total-sin').text(`${items.length} sin contrato`);
    }

    function cargarActivos() {
        const $tb = $('#tbody-contratos');
        $tb.html(loadingRow(7));
        $('#txt-total').text('');
        $('#paginador').empty();

        $.get(URLS.listarActivos, resp => {
            if (!resp || !resp.consultaExitosa) {
                $tb.html(emptyRow(7, resp?.mensaje || 'No se pudieron obtener los contratos activos'));
                return;
            }
            Cache.activos = resp.data || [];
            aplicarFiltro();
        }).fail(() => $tb.html(emptyRow(7, 'Error de conexión')));
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
    function cargarAreas(selector, selectedId) {
        const $select = $(selector || '#nc_area_id');
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

                if (selectedId != null) {
                    $select.val(String(selectedId));
                }
            })
            .fail(function () {
                console.error('Error al cargar áreas');
            });
    }


    // ---------- Cargar Cargos (nuevo / editar) ----------
    function cargarCargos(selector, selectedId) {
        const $select = $(selector || '#nc_cargo_id');
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

                if (selectedId != null) {
                    $select.val(String(selectedId));
                }
            })
            .fail(function () {
                console.error('Error al cargar cargos');
            });
    }

    // ---------- Cargar Pensiones ----------
    function cargarPensiones(selector, selectedId) {
        const $select = $(selector || '#nc_tipo_pension_id');
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

                if (selectedId != null) {
                    $select.val(String(selectedId));
                }
            })
            .fail(function () {
                console.error('Error al cargar pensiones');
            });
    }


    // ---------- Cargar Tipos de Salario (nuevo / editar) ----------
    function cargarTiposSalarios(selector, selectedId) {
        const $select = $(selector || '#nc_tipo_salario_id');
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

                if (selectedId != null) {
                    $select.val(String(selectedId));
                }
            })
            .fail(function () {
                console.error('Error al cargar tipos de salario');
            });
    }


    // ---------- Cargar jornadas ----------
    function cargarJornadas(selector, selectedId) {
        const $select = $(selector || '#nc_tipo_jornada_id');
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

                // Para NUEVO contrato sigues dejando por defecto 1
                if (selector === undefined || selector === '#nc_tipo_jornada_id') {
                    if (selectedId != null) {
                        $select.val(String(selectedId));
                    } else {
                        $select.val("1");
                    }
                } else if (selectedId != null) {
                    $select.val(String(selectedId));
                }
            })
            .fail(function () {
                console.error('Error al cargar tipos de jornada');
            });
    }


    // ===== Confirmar creación de contrato =====
    // ===== Confirmar creación de contrato =====
    $(document).on('click', '#nc_confirmar', function () {

        const contrato = {
            TrabajadorId: Number($('#nc_trabajador_id').val()),
            CargoId: $('#nc_cargo_id').val() ? Number($('#nc_cargo_id').val()) : null,
            AreaId: $('#nc_area_id').val() ? Number($('#nc_area_id').val()) : null,
            TipoPensionId: $('#nc_tipo_pension_id').val() ? Number($('#nc_tipo_pension_id').val()) : null,
            TipoSalarioId: $('#nc_tipo_salario_id').val() ? Number($('#nc_tipo_salario_id').val()) : null,
            TipoJornadaId: $('#nc_tipo_jornada_id').val() ? Number($('#nc_tipo_jornada_id').val()) : 1,

            FechaInicio: $('#nc_fecha_inicio').val(),
            FechaFin: $('#nc_fecha_fin').val() || null,

            Salario: $('#nc_remuneracion').val() ? parseFloat($('#nc_remuneracion').val()) : null,
            HorasSemanales: $('#nc_horas_semanales').val()
                ? parseInt($('#nc_horas_semanales').val(), 10)
                : null,

            TarifaHora: $('#nc_tarifa_hora').val()
                ? parseFloat($('#nc_tarifa_hora').val())
                : null,

            ModoPago: $('#nc_modo_pago').val() || null,
            DescripcionFunciones: $('#nc_descripcion_funciones').val() || null,
            Observaciones: $('#nc_observaciones').val() || null
        };

        // ---------- VALIDACIONES EN CLIENTE ----------

        // Campos obligatorios
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

        // Salario válido (> 0)
        if (!contrato.Salario || isNaN(contrato.Salario) || contrato.Salario <= 0) {
            $('#nc_mensaje').text('Ingrese un salario mayor a 0.');
            return;
        }

        // Horas semanales válidas (> 0)
        if (!contrato.HorasSemanales || isNaN(contrato.HorasSemanales) || contrato.HorasSemanales <= 0) {
            $('#nc_mensaje').text('Ingrese las horas semanales (mayores a 0).');
            return;
        }

        // Validar fechas (fin >= inicio si hay fecha fin)
        const fIni = contrato.FechaInicio ? new Date(contrato.FechaInicio) : null;
        const fFin = contrato.FechaFin ? new Date(contrato.FechaFin) : null;

        if (fIni && fFin && fFin < fIni) {
            $('#nc_mensaje').text('La fecha de fin no puede ser anterior a la fecha de inicio.');
            return;
        }

        // --------------------------------------------
        $('#nc_mensaje').text('Guardando contrato...');

        $.ajax({
            url: URLS.crearContrato,
            type: 'POST',
            data: JSON.stringify(contrato),
            contentType: 'application/json; charset=utf-8',
            success: function (resp) {
                if (resp && resp.exito) {
                    $('#nc_mensaje').text('Contrato creado correctamente.');
                    setTimeout(function () {
                        $('#nc_mensaje').text('');
                        if (window.ContratosUI) {
                            window.ContratosUI.recargarActivos();
                            window.ContratosUI.recargarSin && window.ContratosUI.recargarSin();
                            window.ContratosUI.recargarResumen && window.ContratosUI.recargarResumen();
                        }

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


    // ===== Click en "Nuevo Contrato" desde TAB SIN CONTRATO =====
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
        $('#nc_horas_semanales').val('48');

        cargarAreas();
        cargarCargos();
        cargarPensiones();
        cargarTiposSalarios();
        cargarJornadas();

        openModal('modal-nuevo-contrato');
        setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
    });

    // ---------- Recalcular tarifa hora ----------
    function recalcularTarifaHora() {
        const salario = parseFloat($('#nc_remuneracion').val());
        const horas = parseInt($('#nc_horas_semanales').val(), 10);

        if (!salario || !horas || horas <= 0) {
            $('#nc_tarifa_hora').val('');
            return;
        }

        const jornadaDiaria = horas / 6.0;
        if (jornadaDiaria <= 0) {
            $('#nc_tarifa_hora').val('');
            return;
        }

        const tarifa = salario / (30.0 * jornadaDiaria);
        $('#nc_tarifa_hora').val(tarifa.toFixed(2));
    }

    $(document).on('input', '#nc_remuneracion, #nc_horas_semanales', recalcularTarifaHora);

    // ===== Abrir modal Editar desde contratos activos =====
    $(document).on('click', '.btn-editar-contrato', function () {
        const contratoId = Number($(this).data('contratoid'));
        const item = (Cache.activos || []).find(x => Number(x.ContratoId) === contratoId);
        if (!item) return;

        $('#ec_contrato_id').val(contratoId);
        $('#ec_trabajador_id').val(item.TrabajadorId || '');
        $('#ec_empleado').val(item.EmpleadoNombre || '');
        $('#ec_motivo').val('');
        $('#ec_mensaje').text('');

        // Inputs simples
        $('#ec_salario').val(item.Salario || '');
        $('#ec_modo_pago').val(item.ModoPago || '');
        $('#ec_horas_semanales').val(item.HorasSemanales || '');
        $('#ec_fecha_inicio').val(item.FechaInicio || '');
        $('#ec_fecha_fin').val(item.FechaFin || '');
        $('#ec_tarifa_hora').val(item.TarifaHora || '');
        $('#ec_descripcion_funciones').val(item.DescripcionFunciones || '');
        $('#ec_observaciones').val(item.Observaciones || '');

        // Llenar combos + seleccionar el valor de ese contrato
        cargarAreas('#ec_area_id', item.AreaId);
        cargarPensiones('#ec_tipo_pension_id', item.TipoPensionId);
        cargarCargos('#ec_cargo_id', item.CargoId);
        cargarTiposSalarios('#ec_tipo_salario_id', item.TipoSalarioId);
        cargarJornadas('#ec_tipo_jornada_id', item.TipoJornadaId);

        openModal('modal-editar-contrato');
    });


    // ===== Guardar cambios del contrato =====
    $(document).on('click', '#ec_confirmar', function () {
        const contratoId = Number($('#ec_contrato_id').val());
        const motivo = $('#ec_motivo').val().trim();

        if (!motivo) {
            $('#ec_mensaje').text('Ingrese el motivo de la actualización.');
            return;
        }

        const data = {
            ContratoId: contratoId,
            TrabajadorId: Number($('#ec_trabajador_id').val()),
            Motivo: motivo,

            CargoId: $('#ec_cargo_id').val() ? Number($('#ec_cargo_id').val()) : null,
            AreaId: $('#ec_area_id').val() ? Number($('#ec_area_id').val()) : null,
            TipoPensionId: $('#ec_tipo_pension_id').val() ? Number($('#ec_tipo_pension_id').val()) : null,
            TipoSalarioId: $('#ec_tipo_salario_id').val() ? Number($('#ec_tipo_salario_id').val()) : null,
            TipoJornadaId: $('#ec_tipo_jornada_id').val() ? Number($('#ec_tipo_jornada_id').val()) : null,

            FechaInicio: $('#ec_fecha_inicio').val(),
            FechaFin: $('#ec_fecha_fin').val() || null,

            HorasSemanales: $('#ec_horas_semanales').val()
                ? parseInt($('#ec_horas_semanales').val(), 10)
                : null,

            Salario: $('#ec_salario').val()
                ? parseFloat($('#ec_salario').val())
                : null,

            TarifaHora: $('#ec_tarifa_hora').val()
                ? parseFloat($('#ec_tarifa_hora').val())
                : null,

            ModoPago: $('#ec_modo_pago').val() || null,
            DescripcionFunciones: $('#ec_descripcion_funciones').val() || null,
            Observaciones: $('#ec_observaciones').val() || null
        };

        // ---------- VALIDACIONES EN CLIENTE (EDICIÓN) ----------

        if (data.Salario == null || isNaN(data.Salario) || data.Salario <= 0) {
            $('#ec_mensaje').text('Ingrese un salario mayor a 0.');
            return;
        }

        if (data.HorasSemanales == null || isNaN(data.HorasSemanales) || data.HorasSemanales <= 0) {
            $('#ec_mensaje').text('Ingrese las horas semanales (mayores a 0).');
            return;
        }

        const fIni = data.FechaInicio ? new Date(data.FechaInicio) : null;
        const fFin = data.FechaFin ? new Date(data.FechaFin) : null;

        if (fIni && fFin && fFin < fIni) {
            $('#ec_mensaje').text('La fecha de fin no puede ser anterior a la fecha de inicio.');
            return;
        }

        // -------------------------------------------------------
        $('#ec_mensaje').text('Guardando cambios...');

        $.ajax({
            url: URLS.actualizarContrato,
            type: 'POST',
            data: data,
            success: function (resp) {
                if (resp && resp.exito) {
                    $('#ec_mensaje').text('Contrato actualizado correctamente.');
                    if (window.ContratosUI) {
                        window.ContratosUI.recargarActivos();
                    }
                    setTimeout(function () {
                        $('#ec_mensaje').text('');
                        $('[data-modal-close="modal-editar-contrato"]').click();
                    }, 700);
                } else {
                    $('#ec_mensaje').text(resp && resp.mensaje
                        ? resp.mensaje
                        : 'No se pudo actualizar el contrato.');
                }
            },
            error: function () {
                $('#ec_mensaje').text('Error de conexión al actualizar el contrato.');
            }
        });
    });


    function cargarResumenContratos() {
        $.get(URLS.resumenContratos, function (resp) {
            if (!resp || !resp.exito || !resp.data) {
                console.error('No se pudo obtener el resumen de contratos:', resp && resp.mensaje);
                return;
            }

            const r = resp.data;

            $('#cr_total_contratos').text(r.TotalContratos);
            $('#cr_contratos_activos').text(r.ContratosActivos);
            $('#cr_por_vencer_30').text(r.PorVencer30);
            $('#cr_alertas_legales').text(r.AlertasLegales);
        }).fail(function () {
            console.error('Error de conexión al obtener el resumen de contratos');
        });
    }


    // ---------- Primera carga ----------

    $(function () {
        cargarResumenContratos();
        cargarActivos();
    });


    // Exponer recargas globales
    window.ContratosUI = {
        recargarActivos: cargarActivos,
        recargarSin: cargarSin,
        recargarResumen: cargarResumenContratos
    };

})(jQuery);
