// nomina.ui.js (refactor - procesamiento por trabajador, concurrency, retries)
const NominaUI = (function () {
    'use strict';

    // ===== ESTADO PRIVADO =====
    let datosEmpleadosVigentes = [];
    let datosFiltrados = [];
    let periodoSeleccionado = null;

    // ===== CONTROL DEL PROCESO =====
    let procesoInicio = null;
    let procesoTotalEmpleados = 0;
    let procesoCancelado = false;
    let procesoEnEjecucion = false;
    let activeXhrs = []; // peticiones activas para abort
    const CONCURRENCY = 5;
    const RETRY_ATTEMPTS = 3;
    const RETRY_BACKOFF_MS = 500;

    // ===== REFERENCIAS DOM =====
    const elements = {
        ddlPeriodo: null,
        btnProcesar: null,
        txtBuscarEmpleado: null,
        tblVigentesBody: null,
        tblResumenBody: null,
        tblErroresBody: null,
        kpiPendientes: null,
        kpiProcesadas: null,
        kpiInactivos: null,
        kpiTotalNomina: null,
        modalConfirmar: null,
        modalDetalleEmpleado: null,
        modalProcesando: null,
        btnCancelarProceso: null,
        procTituloPeriodo: null,
        procSubtituloPeriodo: null,
        procPorcentajeTexto: null,
        procBarra: null,
        procTiempoTranscurrido: null,
        procTiempoRestante: null,
        procLogLista: null,
        procKpiProcesados: null,
        procKpiAdvertencias: null,
        procKpiErrores: null,
        procKpiPendientes: null
    };

    // ===== INICIALIZACIÓN =====
    function init() {
        cacheElements();
        bindEvents();
        cargarDatosIniciales();
    }

    function cacheElements() {
        elements.ddlPeriodo = $('#ddlPeriodo');
        elements.btnProcesar = $('#btnProcesar');
        elements.txtBuscarEmpleado = $('#txtBuscarEmpleado');
        elements.tblVigentesBody = $('#tblVigentesBody');
        elements.tblResumenBody = $('#tblResumenBody');
        elements.tblErroresBody = $('#tblErroresBody');
        elements.kpiPendientes = $('#kpiPendientes');
        elements.kpiProcesadas = $('#kpiProcesadas');
        elements.kpiInactivos = $('#kpiInactivos');
        elements.kpiTotalNomina = $('#kpiTotalNomina');
        elements.modalConfirmar = $('#modalConfirmarProcesamiento');
        elements.modalDetalleEmpleado = $('#modalDetalleEmpleado');
        elements.modalProcesando = $('#modalProcesandoNomina');
        elements.btnCancelarProceso = $('#btnCancelarProceso');
        elements.procTituloPeriodo = $('#proc-titulo-periodo');
        elements.procSubtituloPeriodo = $('#proc-subtitulo-periodo');
        elements.procPorcentajeTexto = $('#proc-porcentaje-texto');
        elements.procBarra = $('#proc-barra');
        elements.procTiempoTranscurrido = $('#proc-tiempo-transcurrido');
        elements.procTiempoRestante = $('#proc-tiempo-restante');
        elements.procLogLista = $('#proc-log-lista');
        elements.procKpiProcesados = $('#proc-kpi-procesados');
        elements.procKpiAdvertencias = $('#proc-kpi-advertencias');
        elements.procKpiErrores = $('#proc-kpi-errores');
        elements.procKpiPendientes = $('#proc-kpi-pendientes');
    }

    function bindEvents() {
        elements.ddlPeriodo.on('change', onPeriodoChange);
        elements.btnProcesar.on('click', onProcesarClick);
        if (elements.txtBuscarEmpleado.length) elements.txtBuscarEmpleado.on('input', onBuscarInput);
        if (elements.btnCancelarProceso && elements.btnCancelarProceso.length)
            elements.btnCancelarProceso.on('click', onCancelarProcesoClick);
        $('.btn-close, .btn-cancelar').on('click', cerrarModales);
        $('.modal-overlay').on('click', onModalOverlayClick);
        $(document).on('keydown', onEscapeKey);
    }

    // ===== EVENT HANDLERS =====
    function onPeriodoChange() {
        const rawValue = elements.ddlPeriodo.val();
        periodoSeleccionado = Number(rawValue?.trim());

        if (periodoSeleccionado > 0) cargarEmpleadosVigentes();
        else limpiarTablaVigentes();
    }

    function onProcesarClick() {
        if (!periodoSeleccionado) { Alertas.validacion('Debe seleccionar un período'); return; }
        if (procesoEnEjecucion) { Alertas.info('Ya hay un proceso en ejecución'); return; }
        mostrarModalConfirmacion();
    }

    function onBuscarInput() { filtrarEmpleados(elements.txtBuscarEmpleado.val()); }
    function onModalOverlayClick(e) {
        if (!$(e.target).is('.modal-overlay')) return;
        if ($(e.target).attr('id') === 'modalProcesandoNomina') return;
        cerrarModales();
    }
    function onEscapeKey(e) { if (e.key === 'Escape') cerrarModales(); }

    // ===== DATOS =====
    function cargarDatosIniciales() {
        NominaService.listarPeriodos()
            .done(r => { if (r.consultaExitosa) renderizarSelectPeriodos(r.data); });
        NominaKPIs.init(elements);
    }

    function cargarEmpleadosVigentes() {
        if (!periodoSeleccionado) return limpiarTablaVigentes();
        mostrarCargando();
        NominaService.obtenerEmpleadosVigentes(periodoSeleccionado)
            .done(function (response) {
                const empleados = (response && response.data) || [];
                datosEmpleadosVigentes = empleados;
                datosFiltrados = empleados.slice();
                renderizarTabla(empleados);
                actualizarResumen(empleados);
                elements.btnProcesar.prop('disabled', empleados.length === 0);
            })
            .fail(function () { Alertas.error('Error al cargar empleados'); limpiarTablaVigentes(); });
    }

    // ===== UI helpers (mantener tus originales) =====
    function renderizarSelectPeriodos(periodos) {
        elements.ddlPeriodo.empty().append('<option value="">-- Seleccione un período --</option>');
        (periodos || []).forEach(p => {
            const id = p.Id || p.PeriodoId;
            const nombre = p.Nombre || p.PeriodoNombre || p.Descripcion || '';
            elements.ddlPeriodo.append(`<option value="${id}">${escapeHtml(nombre)}</option>`);
        });
    }
    function renderizarTabla(data) { elements.tblVigentesBody.empty(); if (!data || !data.length) return mostrarEstadoVacio(); data.forEach((item, idx) => elements.tblVigentesBody.append(crearFilaEmpleado(item, idx))); }
    function crearFilaEmpleado(item) {
        const nombre = escapeHtml(item.PersonaNombre || '');
        const apellido = escapeHtml(item.PersonaApellido || '');
        const salario = item.ContratoSalario
            ? `S/ ${formatearMoneda(item.ContratoSalario)}`
            : '—';

        const tipoPension = obtenerTipoPension(item.TipoPensionId);

        const asignacion = item.TieneAsignacionFamiliar
            ? '<span class="badge af">Sí</span>'
            : '<span class="badge af-no">No</span>';

        const cargo = escapeHtml(item.CargoNombre || '—');
        const area = escapeHtml(item.AreaNombre || '—');

        const estadoContrato = renderEstadoContrato(item.EstadoContratoId, item.EstadoContratoNombre);

        // ====== PROCESADO ======
        const procesado = item.Procesado === true || item.Procesado === 1;

        const estadoProcesado = procesado
            ? `<span class="estado-badge estado-procesado">✔ procesado</span>`
            : `<span class="estado-badge estado-no-procesado">⚠ pendiente</span>`;

        const fechaInicio = formatearFecha(item.PeriodoFechaInicio);
        const fechaFin = item.PeriodoFechaFin ? formatearFecha(item.PeriodoFechaFin) : '—';

        return `
        <tr>
            <td>${nombre}</td>
            <td>${apellido}</td>
            <td class="t-center">${salario}</td>
            <td class="t-center">${tipoPension}</td>
            <td class="t-center">${asignacion}</td>
            <td class="t-center">${cargo}</td>
            <td class="t-center">${area}</td>
            <td class="t-center">${estadoContrato}</td>
            <td class="t-center">${fechaInicio}</td>
            <td class="t-center">${fechaFin}</td>
            <td class="t-center">${estadoProcesado}</td>
        </tr>`;
    }

    function obtenerTipoPension(id) {
        const map = {
            1: 'ONP',
            2: 'AFP Integra',
            3: 'AFP Prima',
            4: 'AFP Habitat',
            5: 'AFP Profuturo'
        };
        return map[id] || '—';
    }

    function renderEstadoContrato(estadoId, nombre) {
        const n = escapeHtml(nombre || '—');
        switch (estadoId) {
            case 1: return `<span class="badge badge-activo">${n}</span>`;
            case 2: return `<span class="badge badge-suspendido">${n}</span>`;
            case 3: return `<span class="badge badge-cesado">${n}</span>`;
            default: return `<span class="badge badge-desconocido">${n}</span>`;
        }
    }

    function formatearFecha(fecha) {
        if (!fecha) return '—';

        try {
            // soporta formato /Date(1234567890000)/
            const match = /\/Date\((\d+)\)\//.exec(fecha);
            const ms = match ? parseInt(match[1]) : Date.parse(fecha);

            if (isNaN(ms)) return fecha;

            const d = new Date(ms);
            const yyyy = d.getFullYear();
            const mm = String(d.getMonth() + 1).padStart(2, '0');
            const dd = String(d.getDate()).padStart(2, '0');

            return `${yyyy}-${mm}-${dd}`;
        } catch {
            return fecha;
        }
    }
    function mostrarCargando() { elements.tblVigentesBody.html(`<tr><td colspan="12">Cargando...</td></tr>`); }
    function mostrarEstadoVacio() { elements.tblVigentesBody.html(`<tr><td colspan="12">Seleccione un período</td></tr>`); }
    function limpiarTablaVigentes() { datosEmpleadosVigentes = []; datosFiltrados = []; mostrarEstadoVacio(); }
    function actualizarResumen(empleados) { if (!elements.tblResumenBody) return; const total = empleados.length; const suma = empleados.reduce((s, e) => s + (e.ContratoSalario || 0), 0); elements.tblResumenBody.html(`<tr><td>Total empleados</td><td>${total}</td></tr><tr><td>Suma salarios</td><td>S/ ${formatearMoneda(suma)}</td></tr>`); }

    function filtrarEmpleados(filtro) {
        const texto = (filtro || '').toLowerCase().trim();
        if (!texto) { datosFiltrados = datosEmpleadosVigentes.slice(); renderizarTabla(datosFiltrados); return; }
        datosFiltrados = datosEmpleadosVigentes.filter(e => ((e.PersonaNombre || '') + ' ' + (e.PersonaApellido || '')).toLowerCase().includes(texto) || (e.NumeroIdentificacion || '').toLowerCase().includes(texto));
        renderizarTabla(datosFiltrados);
    }

    // ===== PROCESAMIENTO (nuevo flujo por trabajador) =====
    async function procesarNomina() {
        if (!periodoSeleccionado) return Alertas.validacion('Periodo inválido');
        procesoEnEjecucion = true;
        procesoCancelado = false;

        try {
            // 1) iniciar proceso en servidor -> devuelve nominaId y lista pendientes (ideal)
            agregarLogProceso('Solicitando inicio de proceso en servidor...', 'info');
            const inicioResp = await ajaxPromise(NominaService.iniciarProceso(periodoSeleccionado));
            if (!inicioResp || !inicioResp.ok) {
                agregarLogProceso('No se pudo iniciar el proceso: ' + (inicioResp && inicioResp.msg || 'error'), 'error');
                return;
            }

            const nominaId = inicioResp.nominaId || inicioResp.data && inicioResp.data.nominaId;
            // espera que servidor devuelva lista de trabajadores pendientes; si no, usamos lista local filtrada
            let trabajadores = inicioResp.trabajadores || inicioResp.data && inicioResp.data.trabajadores;
            if (!Array.isArray(trabajadores) || trabajadores.length === 0) {
                trabajadores = ObtenerListaTrabajadoresDesdeUI();
            }

            procesoTotalEmpleados = trabajadores.length;
            procesoInicio = new Date();
            iniciarModalProcesando(elements.ddlPeriodo.find('option:selected').text(), procesoTotalEmpleados);

            // 2) procesar con concurrency + retries
            const resultados = await runQueueWithConcurrency(trabajadores, CONCURRENCY, worker => processWorkerWithRetries(nominaId, worker));

            // 3) resumen y cierre (llamar cerrarProceso)
            const resumen = summarizeResults(resultados);
            agregarLogProceso('Finalizando proceso en servidor...', 'info');
            await ajaxPromise(NominaService.cerrarProceso(nominaId, periodoSeleccionado, resumen.huboErrores));
            finalizarConResumen(resumen);
            cargarEmpleadosVigentes();
        } catch (err) {
            if (!procesoCancelado) {
                agregarLogProceso('Error crítico: ' + (err && err.message || err), 'error');
                finalizarSimulacionError();
            } else {
                agregarLogProceso('Proceso cancelado por usuario.', 'warning');
            }
        } finally {
            procesoEnEjecucion = false;
            activeXhrs.forEach(x => { try { x.abort(); } catch (e) { } });
            activeXhrs = [];
        }
    }

    function ObtenerListaTrabajadoresDesdeUI() {
        // mapea tus datos a { trabajadorId, nombre, contratoId } según lo que necesites
        return datosEmpleadosVigentes
            .filter(c => c.TrabajadorId)
            .map(c => ({ trabajadorId: c.TrabajadorId, nombre: construirNombreCompleto(c), contratoId: c.ContratoId }));
    }

    function runQueueWithConcurrency(items, concurrency, iteratorFn) {
        return new Promise((resolve) => {
            const results = [];
            let index = 0;
            let active = 0;

            function next() {
                if (procesoCancelado) return resolve(results);
                if (index >= items.length && active === 0) return resolve(results);

                while (active < concurrency && index < items.length) {
                    const current = items[index++];
                    active++;
                    Promise.resolve(iteratorFn(current))
                        .then(res => results.push({ item: current, ok: true, result: res }))
                        .catch(err => results.push({ item: current, ok: false, error: err }))
                        .finally(() => { active--; updateProgress(results.length, items.length); next(); });
                }
            }
            next();
        });
    }

    function updateProgress(completed, total) {
        const porcentaje = total === 0 ? 0 : Math.round((completed / total) * 100);
        elements.procBarra.css('width', `${porcentaje}%`);
        elements.procPorcentajeTexto.text(`${completed} de ${total} empleados (${porcentaje}%)`);

        const segTrans = Math.floor((new Date() - procesoInicio) / 1000);
        elements.procTiempoTranscurrido.text(`Tiempo transcurrido: ${segTrans}s`);
        const restante = Math.max(0, Math.round((segTrans / Math.max(1, completed)) * (total - completed)));
        elements.procTiempoRestante.text(`Tiempo estimado restante: ${restante}s`);
        elements.procKpiProcesados.text(completed);
        elements.procKpiPendientes.text(Math.max(0, total - completed));
    }

    function processWorkerWithRetries(nominaId, worker) {
        const trabajadorId = worker.trabajadorId || worker.TrabajadorId || worker.id;
        let attempts = 0;

        return new Promise((resolve, reject) => {
            function attempt() {
                if (procesoCancelado) return reject('cancelled');

                attempts++;
                const jq = NominaService.procesarTrabajador(nominaId, trabajadorId, periodoSeleccionado);
                activeXhrs.push(jq);

                jq.done(resp => {
                    // limpiar XHR de activeXhrs
                    activeXhrs = activeXhrs.filter(x => x !== jq);

                    if (resp && resp.ok) {
                        agregarLogProceso(`OK: ${worker.nombre || trabajadorId}`, 'success');
                        resolve(resp);
                    } else {
                        const msg = (resp && (resp.msg || resp.message)) || 'Error servidor';
                        agregarLogProceso(`WARN/ERR ${worker.nombre || trabajadorId}: ${msg}`, 'warning');
                        // si backend devolvió "incidencia" considerar éxito con advertencia
                        if (resp && resp.incidencia) return resolve(resp);
                        if (attempts < RETRY_ATTEMPTS) {
                            setTimeout(attempt, RETRY_BACKOFF_MS * attempts);
                        } else {
                            reject(msg);
                        }
                    }
                }).fail((xhr, status, err) => {
                    activeXhrs = activeXhrs.filter(x => x !== jq);
                    if (procesoCancelado) return reject('cancelled');
                    if (attempts < RETRY_ATTEMPTS) {
                        setTimeout(attempt, RETRY_BACKOFF_MS * attempts);
                    } else {
                        const texto = (xhr && xhr.responseJSON && xhr.responseJSON.msg) || err || 'Error conexión';
                        agregarLogProceso(`ERROR: ${worker.nombre || trabajadorId} -> ${texto}`, 'error');
                        reject(texto);
                    }
                });
            }
            attempt();
        });
    }

    function summarizeResults(results) {
        const resumen = { TotalEmpleados: results.length, ProcesadosOk: 0, ConErrores: 0, NoProcesados: 0, huboErrores: false };
        results.forEach(r => {
            if (r.ok) resumen.ProcesadosOk++;
            else resumen.ConErrores++;
        });
        resumen.NoProcesados = resumen.TotalEmpleados - (resumen.ProcesadosOk + resumen.ConErrores);
        resumen.huboErrores = resumen.ConErrores > 0;
        return resumen;
    }

    function finalizarConResumen(resumen) {
        elements.procBarra.css('width', '100%');
        elements.procPorcentajeTexto.text(`${resumen.ProcesadosOk + resumen.ConErrores} de ${resumen.TotalEmpleados} empleados (100%)`);
        elements.procKpiProcesados.text(resumen.ProcesadosOk);
        elements.procKpiErrores.text(resumen.ConErrores);
        elements.procKpiPendientes.text(resumen.NoProcesados);
        agregarLogProceso('Proceso completado. ' + (resumen.huboErrores ? 'Con incidencias.' : 'Éxito total.'), resumen.huboErrores ? 'warning' : 'success');
        setTimeout(() => elements.modalProcesando.removeClass('show'), 1200);
    }

    function finalizarSimulacionError() {
        agregarLogProceso('Error durante el proceso.', 'error');
        elements.procTiempoRestante.text('Tiempo estimado restante: -');
        setTimeout(() => elements.modalProcesando.removeClass('show'), 1500);
    }

    function agregarLogProceso(texto, tipo) {
        if (!elements.procLogLista) return;
        const icono = tipo === 'success' ? '🟢' : tipo === 'warning' ? '🟠' : tipo === 'error' ? '🔴' : '🔵';
        const clase = `log-${tipo || 'info'}`;
        const html = `<li class="${clase}"><span class="log-icon">${icono}</span><span class="log-text">${escapeHtml(texto)}</span></li>`;
        elements.procLogLista.prepend(html);
    }

    function iniciarModalProcesando(nombrePeriodo, totalEmpleados) {
        elements.procTituloPeriodo.text(`Procesando Nómina - ${nombrePeriodo}`);
        elements.procPorcentajeTexto.text(`0 de ${totalEmpleados} empleados (0%)`);
        elements.procBarra.css('width', '0%');
        elements.procTiempoTranscurrido.text('Tiempo transcurrido: 0s');
        elements.procTiempoRestante.text('Tiempo estimado restante: -');
        elements.procLogLista.empty();
        elements.procKpiProcesados.text('0');
        elements.procKpiAdvertencias.text('0');
        elements.procKpiErrores.text('0');
        elements.procKpiPendientes.text(totalEmpleados);
        elements.modalProcesando.addClass('show');
    }

    function onCancelarProcesoClick() {
        if (!procesoEnEjecucion) return;

        procesoCancelado = true;

        // abortar XHR
        activeXhrs.forEach(x => { try { x.abort(); } catch (e) { } });
        activeXhrs = [];

        agregarLogProceso("Cancelando proceso...", "warning");

        // llamar al backend para marcar estado Cancelado (estado 5)
        ajaxPromise(NominaService.cerrarProceso(currentNominaId, periodoSeleccionado, true))
            .finally(() => {
                procesoEnEjecucion = false;
                elements.modalProcesando.removeClass("show");
                Alertas.info("Proceso cancelado. La nómina fue marcada como Cancelada.");
            });
    }

    // ===== UTIL =====
    function ajaxPromise(jq) {
        // convierte jqXHR a Promise con manejo de fail/done
        return new Promise((resolve, reject) => {
            jq.done(resp => resolve(resp)).fail((xhr) => reject(xhr));
        });
    }
    function cerrarModales() {
        // remover cualquier modal visible
        try {
            $('.modal-overlay').removeClass('show');
        } catch (e) {
            console.warn('cerrarModales: error al quitar clase show', e);
        }

        // limpiar estado interno si existe modalProcesando
        try {
            if (elements.modalProcesando && elements.modalProcesando.length) {
                elements.modalProcesando.removeClass('show');
            }
            if (elements.modalConfirmar && elements.modalConfirmar.length) {
                elements.modalConfirmar.removeClass('show');
            }
        } catch (e) {
            console.warn('cerrarModales: error al limpiar modales específicos', e);
        }
    }

    function mostrarModalConfirmacion() {
        if (!elements.modalConfirmar || elements.modalConfirmar.length === 0) {
            console.warn('modalConfirmarProcesamiento no encontrado en DOM');
            return;
        }

        const periodoNombre = elements.ddlPeriodo.find('option:selected').text() || '—';

        // SOLO los pendientes
        const pendientes = datosEmpleadosVigentes.filter(e => !e.Procesado).length;

        elements.modalConfirmar.find('.periodo-nombre').text(periodoNombre);
        elements.modalConfirmar.find('.total-empleados').text(pendientes);

        elements.modalConfirmar.addClass('show');

        elements.modalConfirmar
            .find('.btn-confirmar-procesamiento')
            .off('click')
            .on('click', function (e) {
                e.preventDefault();

                elements.modalConfirmar.removeClass('show');

                iniciarModalProcesando(periodoNombre, pendientes);

                $(this).prop('disabled', true);

                procesarNomina().finally(() => {
                    $(this).prop('disabled', false);
                });
            });
    }

    function construirNombreCompleto(emp) { return ((emp.PersonaNombre || '') + ' ' + (emp.PersonaApellido || '')).trim(); }
    function formatearMoneda(valor) { if (valor == null || isNaN(valor)) return '0.00'; return parseFloat(valor).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ','); }
    function escapeHtml(text) { if (text == null) return ''; return String(text).replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' }[m])); }

    // ===== API PÚBLICA =====
    return {
        init: init,
        cargarEmpleadosVigentes: cargarEmpleadosVigentes,
        filtrarEmpleados: filtrarEmpleados,
        procesarNomina: procesarNomina,
        cerrarModales: cerrarModales
    };

})();

$(document).ready(function () { NominaUI.init(); });
window.NominaUI = NominaUI;
