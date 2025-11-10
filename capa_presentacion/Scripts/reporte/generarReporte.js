const ReporteNomina = (function () {
    'use strict';

    // ===== ESTADO PRIVADO =====
    let datosNomina = [];
    let periodoSeleccionado = null;

    // ===== REFERENCIAS DOM =====
    const elements = {
        periodoSelect: null,
        cargoSelect: null,
        btnGenerar: null,
        tableBody: null,
        recordsCount: null,
        modal: null,
        modalBody: null
    };

    function init() {
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
        elements.modal = $('#detallesModal');
        elements.modalBody = $('#modalBody');
    }

    function bindEvents() {
        // ✅ Consulta automática al cambiar el periodo
        elements.periodoSelect.on('change', consultarReporte);

        // ✅ Reconsulta al cambiar el tipo de trabajador (si ya hay un periodo seleccionado)
        elements.cargoSelect.on('change', function () {
            if (elements.periodoSelect.val()) {
                consultarReporte();
            }
        });

        elements.btnGenerar.on('click', generarReporte);

        // Eventos del modal
        $('.btn-close').on('click', cerrarModal);
        elements.modal.on('click', function (e) {
            if (e.target.id === 'detallesModal') {
                cerrarModal();
            }
        });

        // Cerrar modal con tecla ESC
        $(document).on('keydown', function (e) {
            if (e.key === 'Escape' && elements.modal.hasClass('show')) {
                cerrarModal();
            }
        });
    }

    function cargarDatosIniciales() {
        cargarCargos();
        cargarPeriodos();
    }

    function cargarCargos() {
        $.ajax({
            url: window.AppConfig.urls.obtenerCargos,
            type: 'GET',
            success: function (response) {
                if (response.consultaExitosa) {
                    renderizarSelectCargos(response.data);
                } else {
                    console.error('Error al cargar cargos:', response.mensaje);
                    Alertas.error('Error al cargar tipos de trabajador');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error de conexión:', error);
                Alertas.error('Error de conexión al cargar tipos de trabajador');
            }
        });
    }

    function cargarPeriodos() {
        $.ajax({
            url: window.AppConfig.urls.listarPeriodos,
            type: 'GET',
            success: function (response) {
                if (response.consultaExitosa) {
                    renderizarSelectPeriodos(response.data);
                    Alertas.exito('Períodos cargados correctamente');
                } else {
                    Alertas.error('Error al cargar períodos: ' + response.mensaje);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Alertas.error('Error de conexión al cargar períodos');
            }
        });
    }

    // ===== RENDERIZADO =====
    function renderizarSelectCargos(cargos) {
        elements.cargoSelect.empty();
        elements.cargoSelect.append('<option value="">Todos los tipos</option>');

        cargos.forEach(function (cargo) {
            elements.cargoSelect.append(
                `<option value="${cargo.CargoId}">${escapeHtml(cargo.CargoNombre)}</option>`
            );
        });
    }

    function renderizarSelectPeriodos(periodos) {
        elements.periodoSelect.empty();
        elements.periodoSelect.append('<option value="">Seleccione Mes/Año</option>');

        periodos.forEach(function (periodo) {
            elements.periodoSelect.append(
                `<option value="${periodo.PeriodoId}">${escapeHtml(periodo.PeriodoNombre)}</option>`
            );
        });
    }

    function renderizarTabla(data) {
        datosNomina = data;
        elements.tableBody.empty();

        if (!data || data.length === 0) {
            mostrarEstadoVacio();
            return;
        }

        data.forEach((item, index) => {
            const row = crearFilaTrabajador(item, index);
            elements.tableBody.append(row);
        });
    }

    function crearFilaTrabajador(item, index) {
        return `
            <tr>
                <td class="employee-name">${escapeHtml(item.Nombres || '')}</td>
                <td class="employee-name">${escapeHtml(item.Apellidos || '')}</td>
                <td>${escapeHtml(item.TipoTrabajador || 'N/A')}</td>
                <td style="font-weight: 600; color: #38a169;">S/ ${formatearMoneda(item.NetoPagar)}</td>
                <td>
                    <button class="btn-ver-detalles" onclick="ReporteNomina.verDetalles(${index})">
                        <span>👁️</span> Ver Detalles
                    </button>
                </td>
            </tr>
        `;
    }

    // ===== CONSULTA DE REPORTES =====
    function consultarReporte() {
        const periodoId = elements.periodoSelect.val();
        const cargoId = elements.cargoSelect.val();

        // Si no hay periodo seleccionado, limpiar tabla
        if (!periodoId) {
            mostrarEstadoVacio();
            elements.btnGenerar.prop('disabled', true);
            return;
        }

        periodoSeleccionado = periodoId;
        mostrarCargando();

        const url = construirUrlConsulta(periodoId, cargoId);

        $.ajax({
            url: url,
            type: 'GET',
            success: function (response) {
                if (response.consultaExitosa) {
                    renderizarTabla(response.data);
                    actualizarContador(response.data.length);
                    elements.btnGenerar.prop('disabled', false);

                    if (response.data.length > 0) {
                        Alertas.exito(`Se encontraron ${response.data.length} registros`);
                    } else {
                        Alertas.info('No se encontraron registros para el periodo seleccionado');
                    }
                } else {
                    Alertas.error('Error al consultar: ' + response.mensaje);
                    mostrarEstadoVacio();
                    elements.btnGenerar.prop('disabled', true);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Alertas.error('Error de conexión al consultar reporte');
                mostrarEstadoVacio();
                elements.btnGenerar.prop('disabled', true);
            }
        });
    }

    function construirUrlConsulta(periodoId, cargoId) {
        let url = window.AppConfig.urls.listarNomina + '?periodoId=' + periodoId;
        if (cargoId) {
            url += '&cargoId=' + cargoId;
        }
        return url;
    }

    // ===== MODAL DE DETALLES =====
    function verDetalles(index) {
        const item = datosNomina[index];
        if (!item) {
            Alertas.error('No se encontraron los datos del trabajador');
            return;
        }

        const htmlDetalles = construirHtmlDetalles(item);
        elements.modalBody.html(htmlDetalles);
        elements.modal.addClass('show');
    }

    function construirHtmlDetalles(item) {
        return `
            <div class="details-grid">
                ${crearSeccionPersonal(item)}
                ${crearSeccionLaboral(item)}
                ${crearSeccionHoras(item)}
                ${crearSeccionIngresos(item)}
                ${crearSeccionDescuentos(item)}
                ${crearSeccionOtrosDescuentos(item)}
                ${crearSeccionResumen(item)}
            </div>
        `;
    }

    function crearSeccionPersonal(item) {
        return `
            <div class="detail-section">
                <h4>👤 Información Personal</h4>
                ${crearItemDetalle('Código Trabajador', item.CodigoTrabajador)}
                ${crearItemDetalle('Nombres Completos', item.Nombres)}
                ${crearItemDetalle('Apellidos', item.Apellidos)}
                ${crearItemDetalle('Tipo de Identificación', item.TipoDeIdentificacion)}
                ${crearItemDetalle('Número de Identificación', item.NumeroIdentificacion)}
                ${crearItemDetalle('Sistema de Pensión', item.SistemaPension)}
            </div>
        `;
    }

    function crearSeccionLaboral(item) {
        return `
            <div class="detail-section">
                <h4>💼 Información Laboral</h4>
                ${crearItemDetalle('Tipo de Trabajador', item.TipoTrabajador)}
                ${crearItemDetalle('Fecha Inicio Contrato', formatearFecha(item.FechaInicioContrato))}
                ${crearItemDetalle('Fecha Fin Contrato', formatearFecha(item.FechaFinContrato))}
                ${crearItemDetalle('Tipo de Jornada', item.TipoDeJornadaPactada)}
                ${crearItemDetalle('Horas Semanales Pactadas', formatearNumero(item.HorasSemanalesPactadas))}
                ${crearItemDetalle('Periodo', item.PeriodoNomina)}
            </div>
        `;
    }

    function crearSeccionHoras(item) {
        return `
            <div class="detail-section">
                <h4>⏱️ Horas Trabajadas</h4>
                ${crearItemDetalle('Horas Trabajadas Estimadas', formatearNumero(item.HorasTrabajadasEstimadas))}
                ${crearItemDetalle('Horas Extras Reales', formatearNumero(item.HorasExtrasReales))}
                ${crearItemDetalle('Monto Horas Extras', formatearMoneda(item.MontoHorasExtras), 'money')}
            </div>
        `;
    }

    function crearSeccionIngresos(item) {
        return `
            <div class="detail-section">
                <h4>💵 Ingresos</h4>
                ${crearItemDetalle('Sueldo Básico', formatearMoneda(item.SueldoBasico), 'money')}
                ${crearItemDetalle('Asignación Familiar', formatearMoneda(item.AsignacionFamiliar), 'money')}
                ${crearItemDetalle('Bonos', formatearMoneda(item.MontoBonos), 'money')}
                ${crearItemDetalle('Otros Ingresos', formatearMoneda(item.OtrosIngresos), 'money')}
                ${crearItemDetalle('Total Haberes Bruto', formatearMoneda(item.TotalHaberesBruto), 'money')}
                ${crearItemDetalle('Total Haberes', formatearMoneda(item.TotalHaberes), 'money')}
            </div>
        `;
    }

    function crearSeccionDescuentos(item) {
        return `
            <div class="detail-section">
                <h4>📉 Descuentos y Aportes</h4>
                ${crearItemDetalle('Aporte Sistema Pensión', formatearMoneda(item.AporteSistemaPension), 'money')}
                ${crearItemDetalle('Descuento ONP', formatearMoneda(item.DescuentoONP), 'money')}
                ${crearItemDetalle('Descuento AFP', formatearMoneda(item.DescuentoAFP), 'money')}
                ${crearItemDetalle('Retención Impuesto Renta', formatearMoneda(item.RetencionImpuestoRenta), 'money')}
                ${crearItemDetalle('Aporte EsSalud', formatearMoneda(item.AporteEsSalud), 'money')}
                ${crearItemDetalle('Base Imponible EsSalud', formatearMoneda(item.BaseImponibleEsSalud), 'money')}
            </div>
        `;
    }

    function crearSeccionOtrosDescuentos(item) {
        return `
            <div class="detail-section">
                <h4>💳 Otros Descuentos</h4>
                ${crearItemDetalle('Descuento por Faltas', formatearMoneda(item.DescuentoFaltas), 'money')}
                ${crearItemDetalle('Descuento por Adelantos', formatearMoneda(item.DescuentoAdelantos), 'money')}
                ${crearItemDetalle('Otros Descuentos', formatearMoneda(item.OtrosDescuentos), 'money')}
                ${crearItemDetalle('Total Descuentos', formatearMoneda(item.TotalDescuentos), 'money')}
            </div>
        `;
    }

    function crearSeccionResumen(item) {
        return `
            <div class="detail-section" style="grid-column: 1 / -1; background: linear-gradient(135deg, #e6f7ff 0%, #f0fff4 100%); border-left-color: #38a169;">
                <h4>💰 Resumen Final</h4>
                <div class="detail-item">
                    <span class="detail-label">NETO A PAGAR</span>
                    <span class="detail-value highlight">S/ ${formatearMoneda(item.NetoPagar)}</span>
                </div>
            </div>
        `;
    }

    function crearItemDetalle(label, value, clase = '') {
        const valorFormateado = value || 'N/A';
        const claseExtra = clase ? ` ${clase}` : '';
        return `
            <div class="detail-item">
                <span class="detail-label">${escapeHtml(label)}</span>
                <span class="detail-value${claseExtra}">${escapeHtml(valorFormateado)}</span>
            </div>
        `;
    }

    function cerrarModal() {
        elements.modal.removeClass('show');
    }

    // ===== GENERACIÓN DE REPORTES =====
    function generarReporte() {
        if (!datosNomina || datosNomina.length === 0) {
            Alertas.validacion('No hay datos para generar el reporte. Seleccione un período primero');
            return;
        }

        if (!periodoSeleccionado) {
            Alertas.error('No se ha seleccionado un período válido');
            return;
        }

        Alertas.cargando('Preparando la descarga del reporte...');

        // Cuando esté listo el endpoint en el backend:
        // window.location.href = window.AppConfig.urls.generarExcel + '?periodoId=' + periodoSeleccionado;

        // Simulación temporal (eliminar cuando esté el endpoint):
        setTimeout(() => {
            Alertas.ocultarTodas();
            Alertas.info('La funcionalidad de descarga estará disponible próximamente');
        }, 1500);
    }

    // ===== UTILIDADES UI =====
    function mostrarCargando() {
        elements.tableBody.html(`
            <tr>
                <td colspan="5">
                    <div class="loading">
                        <div class="spinner"></div>
                        <p>Cargando datos...</p>
                    </div>
                </td>
            </tr>
        `);
        actualizarContador(0);
    }

    function mostrarEstadoVacio() {
        elements.tableBody.html(`
            <tr>
                <td colspan="5">
                    <div class="empty-state">
                        <div class="empty-state-icon">📋</div>
                        <p>Seleccione un período para ver los registros.</p>
                    </div>
                </td>
            </tr>
        `);
        actualizarContador(0);
    }

    function actualizarContador(count) {
        const texto = count === 1 ? 'registro' : 'registros';
        elements.recordsCount.text(`${count} ${texto}`);
    }

    // ===== UTILIDADES DE FORMATO =====
    function formatearMoneda(valor) {
        if (valor == null || isNaN(valor)) return '0.00';
        return parseFloat(valor).toFixed(2);
    }

    function formatearNumero(valor) {
        if (valor == null || isNaN(valor)) return 'N/A';
        return parseFloat(valor).toFixed(2);
    }

    function formatearFecha(fecha) {
        if (!fecha) return 'N/A';
        try {
            return new Date(fecha).toLocaleDateString('es-PE', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit'
            });
        } catch (e) {
            return 'N/A';
        }
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

    // ===== API PÚBLICA =====
    return {
        init: init,
        verDetalles: verDetalles,
        consultarReporte: consultarReporte,
        generarReporte: generarReporte,
        cerrarModal: cerrarModal
    };

})();

// Inicializar cuando el documento esté listo
$(document).ready(function () {
    ReporteNomina.init();
});