// nomina.ui.js
const NominaUI = (function () {
    'use strict';

    // ===== ESTADO PRIVADO =====
    let datosEmpleadosVigentes = [];
    let datosFiltrados = [];
    let periodoSeleccionado = null;

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
        modalDetalleEmpleado: null
    };

    // ===== INICIALIZACIÓN =====
    function init() {
        console.log(' Inicializando NominaUI...');

        if (typeof window.NominaConfig === 'undefined') {
            console.error('NominaConfig no está definido');
            Alertas.error('Error de configuración del sistema');
            return;
        }

        cacheElements();

        if (!validarElementos()) {
            console.error('Faltan elementos requeridos del DOM');
            return;
        }

        bindEvents();
        cargarDatosIniciales();

        console.log('NominaUI inicializado correctamente');
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
    }

    function validarElementos() {
        const requeridos = ['ddlPeriodo', 'tblVigentesBody'];

        const faltantes = requeridos.filter(key => !elements[key] || elements[key].length === 0);

        if (faltantes.length > 0) {
            console.warn('⚠️ Elementos faltantes:', faltantes);
        }

        return faltantes.length === 0;
    }

    function bindEvents() {
        elements.ddlPeriodo.on('change', onPeriodoChange);
        elements.btnProcesar.on('click', onProcesarClick);

        if (elements.txtBuscarEmpleado.length > 0) {
            elements.txtBuscarEmpleado.on('input', onBuscarInput);
        }

        $(document).on('click', '.btn-ver-detalle', onVerDetalleClick);

        $('.btn-close, .btn-cancelar').on('click', cerrarModales);
        $('.modal-overlay').on('click', onModalOverlayClick);
        $(document).on('keydown', onEscapeKey);
    }

    // ===== HANDLERS DE EVENTOS =====
    function onPeriodoChange() {
        const periodoId = elements.ddlPeriodo.val();
        periodoSeleccionado = periodoId;

        if (periodoId) {
            console.log('Período seleccionado:', periodoId);
            cargarEmpleadosVigentes();
        } else {
            limpiarTablaVigentes();
            elements.btnProcesar.prop('disabled', true);
        }
    }

    function onProcesarClick() {
        if (window.NominaValidate && !window.NominaValidate.validarPeriodo()) {
            Alertas.validacion('Debe seleccionar un período');
            return;
        }

        if (!periodoSeleccionado) {
            Alertas.validacion('Debe seleccionar un período');
            return;
        }

        mostrarModalConfirmacion();
    }

    function onBuscarInput() {
        const filtro = elements.txtBuscarEmpleado.val();
        filtrarEmpleados(filtro);
    }

    function onVerDetalleClick() {
        const index = $(this).data('index');
        verDetalleEmpleado(index);
    }

    function onModalOverlayClick(e) {
        if ($(e.target).hasClass('modal-overlay')) {
            cerrarModales();
        }
    }

    function onEscapeKey(e) {
        if (e.key === 'Escape') {
            cerrarModales();
        }
    }

    // ===== CARGA DE DATOS =====
    function cargarDatosIniciales() {
        console.log('Cargando datos iniciales...');
        cargarPeriodos();
        NominaKPIs.init(elements);
        cargarEmpleadosVigentes();
    }

    function cargarPeriodos() {
        NominaService
            .listarPeriodos()
            .done(function (response) {
                if (response.consultaExitosa) {
                    renderizarSelectPeriodos(response.data);
                    Alertas.exito('Períodos cargados correctamente');
                } else {
                    Alertas.error('Error al cargar períodos: ' + (response.mensaje || ''));
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Error:', error);
                Alertas.error('Error de conexión al cargar períodos');
            });
    }

    function cargarEmpleadosVigentes() {
        console.log('Cargando empleados vigentes...');

        if (!periodoSeleccionado) {
            console.warn('No hay periodo seleccionado, no se cargan empleados');
            limpiarTablaVigentes();
            elements.btnProcesar.prop('disabled', true);
            return;
        }

        mostrarCargando();

        NominaService
            .obtenerEmpleadosVigentes(periodoSeleccionado)
            .done(function (response) {
                const empleados = (response && response.data) || [];

                console.log(`Empleados cargados: ${empleados.length}`);

                datosEmpleadosVigentes = empleados;
                datosFiltrados = empleados;

                renderizarTabla(empleados);
                actualizarResumen(empleados);
                NominaKPIs.desdeEmpleados(empleados, elements, formatearMoneda);

                elements.btnProcesar.prop('disabled', empleados.length === 0);

                if (empleados.length > 0) {
                    Alertas.exito(`Se encontraron ${empleados.length} empleados vigentes`);
                } else {
                    Alertas.info('No se encontraron empleados para este período');
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Error cargando empleados:', error);
                Alertas.error('Error al cargar empleados');

                limpiarTablaVigentes();
                elements.btnProcesar.prop('disabled', true);
            });
    }

    // ===== RENDERIZADO =====
    function renderizarSelectPeriodos(periodos) {
        elements.ddlPeriodo.empty();
        elements.ddlPeriodo.append('<option value="">-- Seleccione un período --</option>');

        if (!periodos || periodos.length === 0) {
            console.warn('No hay períodos disponibles');
            return;
        }

        periodos.forEach(function (periodo) {
            const id = periodo.Id || periodo.PeriodoId;
            const nombre = periodo.Nombre || periodo.Descripcion || periodo.PeriodoNombre;

            elements.ddlPeriodo.append(
                `<option value="${id}">${escapeHtml(nombre)}</option>`
            );
        });

        if (periodos.length > 0) {
            const primerPeriodo = periodos[0].Id || periodos[0].PeriodoId;
            elements.ddlPeriodo.val(primerPeriodo).trigger('change');
        }
    }

    function renderizarTabla(data) {
        datosEmpleadosVigentes = data;
        elements.tblVigentesBody.empty();

        if (!data || data.length === 0) {
            mostrarEstadoVacio();
            return;
        }

        data.forEach((item, index) => {
            const row = crearFilaEmpleado(item, index);
            elements.tblVigentesBody.append(row);
        });

        console.log(`Tabla renderizada con ${data.length} empleados`);
    }

    function crearFilaEmpleado(item, index) {
        const nombre = item.PersonaNombre || 'Sin nombre';
        const apellidos = item.PersonaApellido || 'Sin apellido';
        const salario = item.ContratoSalario || 0;

        // Si más adelante agregas TipoPensionNombre en el DTO, úsalo aquí
        const tipoPension = item.TipoPensionNombre
            ? item.TipoPensionNombre
            : (item.TipoPensionId ? `Tipo ${item.TipoPensionId}` : 'Sin sistema');

        // Puedes mostrar "Sí/No" o el monto fijo; por ahora texto:
        const asignacionFamiliarTexto = item.TieneAsignacionFamiliar ? 'Sí' : 'No';

        const cargo = item.CargoNombre || 'Sin cargo';
        const area = item.AreaNombre || 'Sin área';
        const estadoContrato = item.EstadoContratoNombre || 'N/A';

        const fechaInicio = formatearFecha(item.PeriodoFechaInicio);
        const fechaFin = formatearFecha(item.PeriodoFechaFin);

        const estadoBadge = crearBadgeEstado(estadoContrato);

        return `
        <tr>
            <td style="padding: 10px 12px;">
                <div style="font-weight: 500; color: #333;">${escapeHtml(nombre)}</div>
            </td>
            <td style="padding: 10px 12px;">
                <div style="font-weight: 500; color: #333;">${escapeHtml(apellidos)}</div>
            </td>
            <td class="t-center" style="font-weight: 500; color: #333;">
                S/ ${formatearMoneda(salario)}
            </td>
            <td class="t-center">
                ${escapeHtml(tipoPension)}
            </td>
            <td class="t-right">
                ${asignacionFamiliarTexto}
            </td>
            <td class="t-right">
                ${escapeHtml(cargo)}
            </td>
            <td class="t-right">
                ${escapeHtml(area)}
            </td>
            <td class="t-right">
                ${estadoBadge}
            </td>
            <td class="t-right">
                ${escapeHtml(fechaInicio)}
            </td>
            <td class="t-right">
                ${escapeHtml(fechaFin)}
            </td>
        </tr>
    `;
    }

    function crearBadgeEstado(estado) {
        const esActivo = estado.toUpperCase() === 'ACTIVO';
        const color = esActivo ? '#2e7d32' : '#c62828';
        const bg = esActivo ? '#e8f5e9' : '#ffebee';

        return `
            <span style="display: inline-block; padding: 4px 12px; border-radius: 12px; 
                         font-size: 11px; font-weight: 500; background: ${bg}; color: ${color};">
                ${escapeHtml(estado)}
            </span>
        `;
    }

    function actualizarResumen(empleados) {
        const totalEmpleados = empleados.length;
        const totalSalarios = empleados.reduce((sum, e) => {
            const salario = e.ContratoSalario || 0;
            return sum + salario;
        }, 0);

        const html = `
        <tr>
            <td style="padding: 10px; font-weight: 600;">Total Empleados Vigentes</td>
            <td style="padding: 10px; text-align: right; font-weight: 600;">${totalEmpleados}</td>
        </tr>
        <tr>
            <td style="padding: 10px; font-weight: 600;">Suma de Salarios</td>
            <td style="padding: 10px; text-align: right; color: #1976d2; font-weight: 600;">
                S/ ${formatearMoneda(totalSalarios)}
            </td>
        </tr>
    `;

        if (elements.tblResumenBody && elements.tblResumenBody.length > 0) {
            elements.tblResumenBody.html(html);
        }
    }


    // ===== FILTRADO =====
    function filtrarEmpleados(filtro) {
        if (!filtro || filtro.trim() === '') {
            datosFiltrados = datosEmpleadosVigentes;
            renderizarTabla(datosEmpleadosVigentes);
            return;
        }

        const filtroLower = filtro.toLowerCase().trim();

        datosFiltrados = datosEmpleadosVigentes.filter(function (emp) {
            const nombreCompleto = construirNombreCompleto(emp).toLowerCase();
            const documento = (emp.NumeroIdentificacion || emp.Documento || '').toLowerCase();
            const estado = (emp.Estado || '').toLowerCase();
            const tipoContrato = (emp.TipoContrato || emp.TipoTrabajador || '').toLowerCase();

            return nombreCompleto.includes(filtroLower) ||
                documento.includes(filtroLower) ||
                estado.includes(filtroLower) ||
                tipoContrato.includes(filtroLower);
        });

        renderizarTabla(datosFiltrados);
        console.log(`Filtrados: ${datosFiltrados.length} de ${datosEmpleadosVigentes.length}`);
    }

    // ===== PROCESAMIENTO =====
    function mostrarModalConfirmacion() {
        if (!periodoSeleccionado) {
            Alertas.validacion('Debe seleccionar un período');
            return;
        }

        const periodoNombre = elements.ddlPeriodo.find('option:selected').text();

        if (elements.modalConfirmar && elements.modalConfirmar.length > 0) {
            elements.modalConfirmar.find('.periodo-nombre').text(periodoNombre);
            elements.modalConfirmar.find('.total-empleados').text(datosEmpleadosVigentes.length);
            elements.modalConfirmar.addClass('show');

            elements.modalConfirmar
                .find('.btn-confirmar-procesamiento')
                .off('click')
                .on('click', procesarNomina);
        } else {
            if (confirm(`¿Está seguro de procesar la nómina para ${periodoNombre}?`)) {
                procesarNomina();
            }
        }
    }

    function procesarNomina() {
        if (!periodoSeleccionado) {
            Alertas.error('No hay período seleccionado');
            return;
        }

        cerrarModales();
        Alertas.cargando('Procesando nómina...');

        elements.btnProcesar.prop('disabled', true);

        NominaService
            .procesarNomina(periodoSeleccionado)
            .done(function (response) {
                console.log('Respuesta procesamiento:', response);

                Alertas.ocultarTodas();

                if (response.ok) {
                    Alertas.exito(response.msg || 'Nómina procesada correctamente');
                    setTimeout(function () {
                        cargarEmpleadosVigentes();
                        NominaKPIs.init(elements);
                    }, 1000);
                } else {
                    Alertas.error(response.msg || 'Error al procesar la nómina');
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Error procesando nómina:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);

                Alertas.ocultarTodas();

                let mensajeError = 'Error de conexión al procesar la nómina';

                try {
                    if (xhr.responseJSON && xhr.responseJSON.msg) {
                        mensajeError = xhr.responseJSON.msg;
                    } else if (xhr.responseText) {
                        const respuesta = JSON.parse(xhr.responseText);
                        mensajeError = respuesta.msg || respuesta.message || mensajeError;
                    }
                } catch (e) {
                    console.warn('No se pudo parsear el error del servidor');
                }

                if (mensajeError.toLowerCase().includes('ya fue procesada') ||
                    mensajeError.toLowerCase().includes('ya existe') ||
                    mensajeError.toLowerCase().includes('en proceso')) {
                    Alertas.validacion(mensajeError);
                } else {
                    Alertas.error(mensajeError);
                }
            })
            .always(function () {
                elements.btnProcesar.prop('disabled', false);
            });
    }

    // ===== ESTADOS UI =====
    function mostrarCargando() {
        elements.tblVigentesBody.html(`
            <tr>
                <td colspan="12" style="padding: 50px; text-align: center;">
                    <div style="display: inline-block;">
                        <div class="spinner" style="border: 3px solid #f3f3f3; border-top: 3px solid #1976d2; 
                                                     border-radius: 50%; width: 40px; height: 40px; 
                                                     animation: spin 1s linear infinite; margin: 0 auto 15px;"></div>
                        <div style="color: #666; font-size: 14px;">Cargando datos...</div>
                    </div>
                </td>
            </tr>
        `);
    }

    function mostrarEstadoVacio() {
        elements.tblVigentesBody.html(`
            <tr>
                <td colspan="12" style="padding: 50px; text-align: center; color: #999;">
                    <div style="font-size: 48px; margin-bottom: 15px;">📋</div>
                    <div style="font-size: 14px;">Seleccione un período para ver empleados</div>
                </td>
            </tr>
        `);
    }

    function limpiarTablaVigentes() {
        mostrarEstadoVacio();
        datosEmpleadosVigentes = [];
        datosFiltrados = [];
    }

    function cerrarModales() {
        $('.modal-overlay').removeClass('show');
    }

    // ===== UTILIDADES =====
    function construirNombreCompleto(emp) {
        const nombres =
            emp.Nombres ||
            emp.Nombre ||
            emp.NombreCompleto ||
            emp.PersonaNombre ||
            '';
        const apellidos =
            emp.Apellidos ||
            emp.PersonaApellido ||
            '';

        if (nombres && apellidos) {
            return `${nombres} ${apellidos}`;
        }
        return nombres || apellidos || 'Sin nombre';
    }

    function formatearMoneda(valor) {
        if (valor == null || isNaN(valor)) return '0.00';
        return parseFloat(valor)
            .toFixed(2)
            .replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }
    function formatearFecha(valor) {
        if (!valor) return '';

        let fecha = null;

        if (typeof valor === 'string') {
            if (valor.indexOf('/Date(') === 0) {
                const ms = parseInt(valor.replace('/Date(', '').replace(')/', ''), 10);
                fecha = new Date(ms);
            } else {
                fecha = new Date(valor);
            }
        } else {
            fecha = new Date(valor);
        }

        if (isNaN(fecha)) return '';

        const d = String(fecha.getDate()).padStart(2, '0');
        const m = String(fecha.getMonth() + 1).padStart(2, '0');
        const y = fecha.getFullYear();

        return `${d}/${m}/${y}`;
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
        cargarEmpleadosVigentes: cargarEmpleadosVigentes,
        filtrarEmpleados: filtrarEmpleados,
        procesarNomina: procesarNomina,
        cerrarModales: cerrarModales
    };

})();

// AUTO-INICIALIZACIÓN
$(document).ready(function () {
    console.log('DOM Ready - Iniciando NominaUI');

    setTimeout(() => {
        Alertas.info('Sistema de nómina cargado', 2000);
    }, 500);

    NominaUI.init();
});

window.NominaUI = NominaUI;
console.log('Módulo NominaUI cargado correctamente');
