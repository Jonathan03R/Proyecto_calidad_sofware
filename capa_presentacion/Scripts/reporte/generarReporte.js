const ReporteNomina = (function () {
    'use strict';

    // ===== CONSTANTES =====
    const COLUMNAS_TABLA = 32;
    const COLUMNAS_CONFIG = {
        CODIGO: { index: 1, label: 'Código' },
        NOMBRES: { index: 2, label: 'Nombres' },
        APELLIDOS: { index: 3, label: 'Apellidos' }
    };

    // ===== ESTADO PRIVADO =====
    const state = {
        datosNomina: [],
        periodoSeleccionado: null
    };

    // ===== REFERENCIAS DOM =====
    const elements = {
        periodoSelect: null,
        cargoSelect: null,
        btnGenerar: null,
        tableBody: null,
        recordsCount: null
    };

    // ===== INICIALIZACIÓN =====
    function init() {
        console.log('🚀 Inicializando ReporteNomina...');
        cacheElements();
        bindEvents();
        cargarDatosIniciales();
    }

    function cacheElements() {
        elements.periodoSelect = $('#periodoId');
        elements.cargoSelect = $('#cargoId');
        elements.btnGenerar = $('#btnGenerarReporte');
        elements.tableBody = $('#tableBody');
        elements.recordsCount = $('#recordsCount');

        console.log('📦 Elementos cacheados');
    }

    function bindEvents() {
        elements.periodoSelect.on('change', handlePeriodoChange);
        elements.cargoSelect.on('change', handleCargoChange);
        elements.btnGenerar.on('click', generarReportePDF);

        console.log('✅ Eventos vinculados');
    }

    // ===== MANEJADORES DE EVENTOS =====
    function handlePeriodoChange() {
        consultarReporte();
    }

    function handleCargoChange() {
        if (elements.periodoSelect.val()) {
            consultarReporte();
        }
    }

    // ===== CARGA DE DATOS INICIALES =====
    function cargarDatosIniciales() {
        cargarCargos();
        cargarPeriodos();
    }

    function cargarCargos() {
        ejecutarAjax({
            url: window.AppConfig.urls.obtenerCargos,
            onSuccess: (response) => {
                if (response.consultaExitosa) {
                    renderizarSelectCargos(response.data);
                } else {
                    manejarError('Error al cargar tipos de trabajador', response.mensaje);
                }
            },
            onError: () => manejarError('Error de conexión al cargar tipos de trabajador')
        });
    }

    function cargarPeriodos() {
        ejecutarAjax({
            url: window.AppConfig.urls.listarPeriodos,
            onSuccess: (response) => {
                if (response.consultaExitosa) {
                    renderizarSelectPeriodos(response.data);
                    Alertas.exito('Períodos cargados correctamente');
                } else {
                    manejarError('Error al cargar períodos', response.mensaje);
                }
            },
            onError: () => manejarError('Error de conexión al cargar períodos')
        });
    }

    // ===== AJAX GENÉRICO =====
    function ejecutarAjax({ url, type = 'GET', onSuccess, onError }) {
        $.ajax({
            url: url,
            type: type,
            success: onSuccess,
            error: function (xhr, status, error) {
                console.error('Error AJAX:', error);
                if (onError) onError(xhr, status, error);
            }
        });
    }

    // ===== RENDERIZADO DE SELECTS =====
    function renderizarSelectCargos(cargos) {
        const opciones = [
            crearOpcion('', 'Todos los tipos'),
            ...cargos.map(cargo => crearOpcion(cargo.CargoId, cargo.CargoNombre))
        ];
        elements.cargoSelect.html(opciones.join(''));
    }

    function renderizarSelectPeriodos(periodos) {
        const opciones = [
            crearOpcion('', 'Seleccione Mes/Año'),
            ...periodos.map(periodo => crearOpcion(periodo.PeriodoId, periodo.PeriodoNombre))
        ];
        elements.periodoSelect.html(opciones.join(''));
    }

    function crearOpcion(valor, texto) {
        return `<option value="${valor}">${escapeHtml(texto)}</option>`;
    }

    // ===== RENDERIZADO DE TABLA =====
    function renderizarTabla(data) {
        state.datosNomina = data;
        elements.tableBody.empty();

        if (!data || data.length === 0) {
            mostrarEstadoVacio();
            return;
        }

        const filas = data.map((item, index) => crearFilaTrabajador(item, index));
        elements.tableBody.html(filas.join(''));
    }

    function crearFilaTrabajador(item, index) {
        const campos = [
            escapeHtml(item.CodigoTrabajador || ''),
            escapeHtml(item.Nombres || ''),
            escapeHtml(item.Apellidos || ''),
            escapeHtml(item.TipoDeIdentificacion || ''),
            escapeHtml(item.NumeroIdentificacion || ''),
            escapeHtml(item.SistemaPension || ''),
            escapeHtml(item.TipoTrabajador || ''),
            escapeHtml(item.FechaInicioContrato || ''),
            escapeHtml(item.FechaFinContrato || ''),
            formatearNumero(item.HorasSemanalesPactadas),
            formatearNumero(item.HorasExtrasReales),
            formatearMonedaConSimbolo(item.SueldoBasico),
            formatearMonedaConSimbolo(item.AsignacionFamiliar),
            formatearMonedaConSimbolo(item.MontoHorasExtras),
            formatearMonedaConSimbolo(item.MontoBonos),
            formatearMonedaConSimbolo(item.OtrosIngresos),
            formatearMonedaConSimbolo(item.TotalHaberesBruto),
            formatearMonedaConSimbolo(item.TotalHaberes),
            formatearMonedaConSimbolo(item.AporteSistemaPension),
            formatearMonedaConSimbolo(item.RetencionImpuestoRenta),
            formatearMonedaConSimbolo(item.AporteEsSalud),
            formatearMonedaConSimbolo(item.BaseImponibleEsSalud),
            formatearMonedaConSimbolo(item.DescuentoTardanzas),
            formatearMonedaConSimbolo(item.DescuentoFaltas),
            formatearMonedaConSimbolo(item.TotalDescuentos),
            `<span class="highlight">${formatearMonedaConSimbolo(item.NetoPagar)}</span>`
        ];

        return `<tr>${campos.map((campo, i) => crearCelda(campo, i + 1)).join('')}</tr>`;
    }

    function crearCelda(contenido, indice) {
        const esNombre = indice === COLUMNAS_CONFIG.NOMBRES.index || indice === COLUMNAS_CONFIG.APELLIDOS.index;
        const esMoneda = indice >= 13 && indice <= 31;
        const clases = [];

        if (esNombre) clases.push('employee-name');
        if (esMoneda) clases.push('money');

        const claseStr = clases.length > 0 ? ` class="${clases.join(' ')}"` : '';
        return `<td${claseStr}>${contenido}</td>`;
    }

    // ===== CONSULTA DE REPORTES =====
    function consultarReporte() {
        const periodoId = elements.periodoSelect.val();
        const cargoId = elements.cargoSelect.val();

        if (!periodoId) {
            resetearVista();
            return;
        }

        state.periodoSeleccionado = periodoId;
        mostrarCargando();

        const url = construirUrlConsulta(periodoId, cargoId);

        ejecutarAjax({
            url: url,
            onSuccess: (response) => manejarRespuestaConsulta(response),
            onError: () => {
                Alertas.error('Error de conexión al consultar reporte');
                resetearVista();
            }
        });
    }

    function construirUrlConsulta(periodoId, cargoId) {
        let url = `${window.AppConfig.urls.listarNomina}?periodoId=${periodoId}`;
        if (cargoId) url += `&cargoId=${cargoId}`;
        return url;
    }

    function manejarRespuestaConsulta(response) {
        if (response.consultaExitosa) {
            renderizarTabla(response.data);
            actualizarContador(response.data.length);
            elements.btnGenerar.prop('disabled', false);

            mostrarMensajeResultados(response.data.length);
        } else {
            Alertas.error('Error al consultar: ' + response.mensaje);
            resetearVista();
        }
    }

    function mostrarMensajeResultados(cantidad) {
        if (cantidad > 0) {
            Alertas.exito(`Se encontraron ${cantidad} registros`);
        } else {
            Alertas.info('No se encontraron registros para el periodo seleccionado');
        }
    }

    function resetearVista() {
        mostrarEstadoVacio();
        elements.btnGenerar.prop('disabled', true);
    }

    // ===== GENERACIÓN DE REPORTE PDF =====
    function generarReportePDF() {
        console.log('📄 Generando reporte PDF...');

        if (!validarDatosParaReporte()) return;

        // Verificar que ExportadorReportes esté disponible
        if (typeof ExportadorReportes === 'undefined') {
            console.error('❌ ExportadorReportes no está disponible');
            Alertas.error('El módulo de exportación no está disponible. Verifique que exportadorReportes.js esté cargado.');
            return;
        }

        const cargoId = elements.cargoSelect.val();
        const periodoInfo = elements.periodoSelect.find(':selected').text();
        const nombreArchivo = `reporte_nomina_${sanitizarNombreArchivo(periodoInfo)}`;

        const urlExportacion = window.AppConfig.urls.generarPDF;

        if (!urlExportacion) {
            Alertas.error('La URL de exportación PDF no está configurada');
            return;
        }

        console.log('📤 Iniciando exportación PDF:', {
            periodoId: state.periodoSeleccionado,
            cargoId: cargoId,
            url: urlExportacion
        });

        ExportadorReportes.exportarPDF({
            url: urlExportacion,
            periodoId: state.periodoSeleccionado,
            cargoId: cargoId || null,
            nombreArchivo: nombreArchivo,
            onStart: () => {
                console.log('⏳ Descarga iniciada...');
                Alertas.cargando('Preparando descarga del PDF...');
                elements.btnGenerar.prop('disabled', true);
            },
            onSuccess: (nombreArchivo) => {
                console.log('✅ Descarga exitosa:', nombreArchivo);
                Alertas.ocultarTodas();
                Alertas.exito('PDF descargado exitosamente');
            },
            onError: (error) => {
                console.error('❌ Error en descarga:', error);
                Alertas.ocultarTodas();
                Alertas.error(`Error: ${error}`);
            },
            onFinally: () => {
                console.log('🏁 Descarga finalizada');
                elements.btnGenerar.prop('disabled', false);
            }
        });
    }

    function validarDatosParaReporte() {
        if (!state.datosNomina || state.datosNomina.length === 0) {
            Alertas.validacion('No hay datos para generar el reporte');
            return false;
        }

        if (!state.periodoSeleccionado) {
            Alertas.error('No se ha seleccionado un período válido');
            return false;
        }

        return true;
    }

    function sanitizarNombreArchivo(nombre) {
        return nombre
            .toLowerCase()
            .replace(/\s+/g, '_')
            .replace(/[^a-z0-9_-]/g, '')
            .substring(0, 50);
    }

    function mostrarCargando() {
        elements.tableBody.html(crearMensajeEstado('loading', '📊', 'Cargando datos...'));
        actualizarContador(0);
    }

    function mostrarEstadoVacio() {
        elements.tableBody.html(crearMensajeEstado('empty-state', '📋', 'Seleccione un período para ver los registros.'));
        actualizarContador(0);
    }

    function crearMensajeEstado(clase, icono, mensaje) {
        return `
            <tr>
                <td colspan="${COLUMNAS_TABLA}">
                    <div class="${clase}">
                        ${clase === 'loading' ? '<div class="spinner"></div>' : `<div class="${clase}-icon">${icono}</div>`}
                        <p>${mensaje}</p>
                    </div>
                </td>
            </tr>
        `;
    }

    function actualizarContador(count) {
        const texto = count === 1 ? 'registro' : 'registros';
        elements.recordsCount.text(`${count} ${texto}`);
    }

    function formatearMoneda(valor) {
        if (valor == null || isNaN(valor)) return '0.00';
        return parseFloat(valor).toFixed(2);
    }

    function formatearMonedaConSimbolo(valor) {
        return `S/ ${formatearMoneda(valor)}`;
    }

    function formatearNumero(valor) {
        if (valor == null || isNaN(valor)) return 'N/A';
        return parseFloat(valor).toFixed(2);
    }

    function escapeHtml(text) {
        if (text == null) return '';
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return String(text).replace(/[&<>"']/g, m => map[m]);
    }

    function manejarError(mensaje, detalle = '') {
        console.error(mensaje, detalle);
        Alertas.error(mensaje);
    }

    // ===== API PÚBLICA =====
    return {
        init,
        consultarReporte,
        generarReportePDF
    };

})();

$(document).ready(() => {
    console.log('📄 DOM Ready');
    ReporteNomina.init();
});