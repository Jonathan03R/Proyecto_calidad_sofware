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
        console.log('🚀 Inicializando NominaUI...');

        if (typeof window.NominaConfig === 'undefined') {
            console.error('❌ NominaConfig no está definido');
            Alertas.error('Error de configuración del sistema');
            return;
        }

        cacheElements();

        if (!validarElementos()) {
            console.error('❌ Faltan elementos requeridos del DOM');
            return;
        }

        bindEvents();
        cargarDatosIniciales();

        console.log('✅ NominaUI inicializado correctamente');
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
        const requeridos = ['ddlPeriodo', 'btnProcesar', 'tblVigentesBody'];
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
            console.log('📅 Período seleccionado:', periodoId);
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
        console.log('📊 Cargando datos iniciales...');
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
        console.log('👥 Cargando empleados vigentes...');

        if (!periodoSeleccionado) {
            console.warn('⚠ No hay periodo seleccionado, no se cargan empleados');
            limpiarTablaVigentes();
            elements.btnProcesar.prop('disabled', true);
            return;
        }

        mostrarCargando();

        NominaService
            .obtenerEmpleadosVigentes(periodoSeleccionado)
            .done(function (response) {
                const empleados = (response && response.data) || [];

                console.log(`✅ Empleados cargados: ${empleados.length}`);

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
                console.error('❌ Error cargando empleados:', error);
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
            console.warn('⚠️ No hay períodos disponibles');
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

        console.log(`✅ Tabla renderizada con ${data.length} empleados`);
    }

    function crearFilaEmpleado(item, index) {
        const nombre = item.PersonaNombre || item.Nombre || 'Sin nombre';
        const apellidos = item.PersonaApellido || item.Apellidos || 'Sin apellido';
        const estado = item.Estado || 'ACTIVO';
        const tipoContrato = item.TipoContrato || item.EstadoContratoNombre || 'N/A';
        const salarioBase = item.SalarioBase || item.SueldoBasico || 0;
        const asigFamiliar = item.AsignacionFamiliar || 0;
        const horasExtras = item.MontoHorasExtras || item.HorasExtras || 0;
        const salarioBruto = item.SalarioBruto || item.TotalHaberesBruto || (salarioBase + asigFamiliar + horasExtras);
        const descuentos = item.TotalDescuentos || 0;
        const salarioNeto = item.SalarioNeto || item.NetoPagar || (salarioBruto - descuentos);

        const estadoBadge = crearBadgeEstado(estado);

        return `
            <tr>
                <td style="padding: 10px 12px;">
                    <div style="font-weight: 500; color: #333;">${escapeHtml(nombre)}</div>
                </td>
                <td style="padding: 10px 12px;">
                    <div style="font-weight: 500; color: #333;">${escapeHtml(apellidos)}</div>
                </td>
                <td style="text-align: center;">${estadoBadge}</td>
                <td style="padding: 10px 12px; text-align: center;">${escapeHtml(tipoContrato)}</td>
                <td style="text-align: right; font-weight: 500; color: #333;">S/ ${formatearMoneda(salarioBase)}</td>
                <td style="text-align: right; color: #666;">S/ ${formatearMoneda(asigFamiliar)}</td>
                <td style="text-align: right; color: #666;">S/ ${formatearMoneda(horasExtras)}</td>
                <td style="text-align: right; font-weight: 600; color: #1976d2;">S/ ${formatearMoneda(salarioBruto)}</td>
                <td style="text-align: right; color: #d32f2f;">S/ ${formatearMoneda(descuentos)}</td>
                <td style="text-align: right; font-weight: 600; color: #2e7d32;">S/ ${formatearMoneda(salarioNeto)}</td>
                <td style="text-align: center;">
                    <button class="btn-ver-detalle" data-index="${index}">
                        <span>👁️</span> Ver
                    </button>
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
        const totalBruto = empleados.reduce((sum, e) => sum + (e.SalarioBruto || e.TotalHaberesBruto || 0), 0);
        const totalDescuentos = empleados.reduce((sum, e) => sum + (e.TotalDescuentos || 0), 0);
        const totalNeto = empleados.reduce((sum, e) => sum + (e.SalarioNeto || e.NetoPagar || 0), 0);

        const html = `
            <tr>
                <td style="padding: 10px; font-weight: 600;">Total Empleados</td>
                <td style="padding: 10px; text-align: right; font-weight: 600;">${totalEmpleados}</td>
            </tr>
            <tr>
                <td style="padding: 10px; font-weight: 600;">Total Salarios Bruto</td>
                <td style="padding: 10px; text-align: right; color: #1976d2; font-weight: 600;">S/ ${formatearMoneda(totalBruto)}</td>
            </tr>
            <tr>
                <td style="padding: 10px; font-weight: 600;">Total Descuentos</td>
                <td style="padding: 10px; text-align: right; color: #d32f2f; font-weight: 600;">S/ ${formatearMoneda(totalDescuentos)}</td>
            </tr>
            <tr style="background: #f8f9fa; border-top: 2px solid #dee2e6;">
                <td style="padding: 12px; font-weight: 700; font-size: 14px;">Total Nómina Neta</td>
                <td style="padding: 12px; text-align: right; color: #2e7d32; font-weight: 700; font-size: 16px;">S/ ${formatearMoneda(totalNeto)}</td>
            </tr>
        `;

        elements.tblResumenBody.html(html);
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
        console.log(`🔍 Filtrados: ${datosFiltrados.length} de ${datosEmpleadosVigentes.length}`);
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
                console.log('✅ Respuesta procesamiento:', response);

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
                console.error('❌ Error procesando nómina:', error);
                console.error('❌ Status:', status);
                console.error('❌ Response:', xhr.responseText);

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

    // ===== DETALLE DE EMPLEADO =====
    function verDetalleEmpleado(index) {
        const empleado = datosFiltrados[index];

        if (!empleado) {
            Alertas.error('No se encontró el empleado');
            return;
        }

        console.log('👁️ Ver detalle de:', construirNombreCompleto(empleado));
        console.log('📊 Datos del empleado:', empleado);

        if (elements.modalDetalleEmpleado && elements.modalDetalleEmpleado.length > 0) {
            const htmlDetalle = construirHtmlDetalle(empleado);
            elements.modalDetalleEmpleado.find('.modal-body').html(htmlDetalle);
            elements.modalDetalleEmpleado.addClass('show');
        } else {
            console.error('❌ Modal no encontrado en el DOM');
            Alertas.error('No se pudo abrir el modal de detalle');
        }
    }

    function construirHtmlDetalle(emp) {
        const sueldoBasico = emp.SueldoBasico || 0;
        const asigFamiliar = emp.AsignacionFamiliar || 0;
        const horasExtras = emp.HorasExtras || 0;
        const bonosRegulares = emp.BonosRegulares || 0;
        const otrosIngresos = emp.OtrosIngresos || 0;
        const remuneracionBruta = emp.RemuneracionBruta || 0;
        const totalIngresos = emp.TotalIngresos || 0;

        const sistemaPension = emp.SistemaPensionAplicado || 'N/A';
        const aporteEssalud = emp.AporteEssalud || 0;
        const aporteOnp = emp.AporteOnp || 0;
        const descuentoAfp = emp.DescuentoAfp || 0;

        const remuneracionAcumuladaAnual = emp.RemuneracionAcumuladaAnual || 0;
        const baseImponibleAnual = emp.BaseImponibleAnual || 0;
        const impuestoRentaAnual = emp.ImpuestoRentaAnual || 0;
        const impuestoRentaMensual = emp.ImpuestoRentaMensual || 0;
        const uitValor = emp.UitValor || 0;
        const deduccion7Uit = emp.Deduccion7Uit || 0;

        const descuentoTardanzas = emp.DescuentoTardanzas || 0;
        const descuentoFaltas = emp.DescuentoFaltas || 0;
        const descuentoAdelantos = emp.DescuentoAdelantos || 0;
        const otrosDescuentos = emp.OtrosDescuentos || 0;

        const totalDescuentos = emp.TotalDescuentos || 0;
        const netoPagar = emp.NetoPagar || 0;

        const totalPensiones = aporteOnp + descuentoAfp;

        return `
            <div style="margin-bottom: 15px;">
                <div style="font-size: 14px; color: #666;">
                    <strong>Nombre:</strong> 
                    <span style="color: #333;">${escapeHtml(emp.Nombre || emp.PersonaNombre || 'Sin nombre')}</span>
                </div>
                <div style="font-size: 14px; color: #666; margin-top: 5px;">
                    <strong>Apellido:</strong> 
                    <span style="color: #333;">${escapeHtml(emp.Apellidos || emp.PersonaApellido || 'Sin apellido')}</span>
                </div>
                <div style="font-size: 14px; color: #666; margin-top: 5px;">
                    <strong>Tipo de Contrato:</strong> 
                    <span style="color: #333;">${escapeHtml(emp.EstadoContratoNombre || emp.TipoContrato || 'N/A')}</span>
                </div>
                <div style="font-size: 14px; color: #666; margin-top: 5px;">
                    <strong>Sistema de Pensión:</strong> 
                    <span style="color: #333;">${escapeHtml(sistemaPension)}</span>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 30px; margin-top: 25px;">
                <div>
                    <h4 style="margin: 0 0 15px 0; color: #333; font-size: 16px; font-weight: 600;">
                        Ingresos 
                        <span style="float: right;">S/ ${formatearMoneda(totalIngresos)}</span>
                    </h4>
                    <div style="font-size: 13px; line-height: 2;">
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Sueldo Básico</span>
                            <span>S/ ${formatearMoneda(sueldoBasico)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Asignación Familiar</span>
                            <span>S/ ${formatearMoneda(asigFamiliar)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Horas Extras</span>
                            <span>S/ ${formatearMoneda(horasExtras)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Bonos Regulares</span>
                            <span>S/ ${formatearMoneda(bonosRegulares)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Otros Ingresos</span>
                            <span>S/ ${formatearMoneda(otrosIngresos)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #333; font-weight: 600; margin-top: 10px; padding-top: 10px; border-top: 1px solid #e0e0e0;">
                            <span>Remuneración Bruta</span>
                            <span>S/ ${formatearMoneda(remuneracionBruta)}</span>
                        </div>
                    </div>
                </div>

                <div>
                    <h4 style="margin: 0 0 15px 0; color: #333; font-size: 16px; font-weight: 600;">
                        Descuentos 
                        <span style="float: right;">S/ ${formatearMoneda(totalDescuentos)}</span>
                    </h4>
                    <div style="font-size: 13px; line-height: 2;">
                        <div style="font-weight: 600; margin-bottom: 5px;">
                            <div style="display: flex; justify-content: space-between; color: #333;">
                                <span>Sistema de Pensiones (${escapeHtml(sistemaPension)})</span>
                                <span>S/ ${formatearMoneda(totalPensiones)}</span>
                            </div>
                        </div>

                        ${aporteOnp > 0 ? `
                        <div style="display: flex; justify-content: space-between; color: #666; padding-left: 15px;">
                            <span>ONP (13%)</span>
                            <span>S/ ${formatearMoneda(aporteOnp)}</span>
                        </div>` : ''}

                        ${descuentoAfp > 0 ? `
                        <div style="display: flex; justify-content: space-between; color: #666; padding-left: 15px;">
                            <span>AFP (Aporte + Comisión + Seguro)</span>
                            <span>S/ ${formatearMoneda(descuentoAfp)}</span>
                        </div>` : ''}

                        <div style="display: flex; justify-content: space-between; color: #666; margin-top: 10px;">
                            <span>Impuesto a la Renta (5ta categoría)</span>
                            <span>S/ ${formatearMoneda(impuestoRentaMensual)}</span>
                        </div>

                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Tardanzas</span>
                            <span>S/ ${formatearMoneda(descuentoTardanzas)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Faltas</span>
                            <span>S/ ${formatearMoneda(descuentoFaltas)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Adelantos de Sueldo</span>
                            <span>S/ ${formatearMoneda(descuentoAdelantos)}</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666;">
                            <span>Otros Descuentos</span>
                            <span>S/ ${formatearMoneda(otrosDescuentos)}</span>
                        </div>

                        <div style="font-weight: 600; margin-top: 10px; padding-top: 10px; border-top: 1px solid #e0e0e0;">
                            <div style="display: flex; justify-content: space-between; color: #333;">
                                <span>Aportes del Empleador</span>
                                <span>S/ ${formatearMoneda(aporteEssalud)}</span>
                            </div>
                        </div>
                        <div style="display: flex; justify-content: space-between; color: #666; padding-left: 15px;">
                            <span>EsSalud (9%)</span>
                            <span>S/ ${formatearMoneda(aporteEssalud)}</span>
                        </div>
                    </div>
                </div>
            </div>

            ${impuestoRentaMensual > 0 ? `
            <div style="margin-top: 25px; padding: 15px; background: #f8f9fa; border-radius: 5px; border-left: 4px solid #1976d2;">
                <h5 style="margin: 0 0 10px 0; color: #333; font-size: 14px;">Detalle Impuesto a la Renta</h5>
                <div style="font-size: 12px; color: #666; line-height: 1.8;">
                    <div style="display: flex; justify-content: space-between;">
                        <span>Remuneración Acumulada Anual:</span>
                        <span>S/ ${formatearMoneda(remuneracionAcumuladaAnual)}</span>
                    </div>
                    <div style="display: flex; justify-content: space-between;">
                        <span>Deducción (7 UIT = S/ ${formatearMoneda(uitValor)} × 7):</span>
                        <span>S/ ${formatearMoneda(deduccion7Uit)}</span>
                    </div>
                    <div style="display: flex; justify-content: space-between; font-weight: 600; color: #333;">
                        <span>Base Imponible Anual:</span>
                        <span>S/ ${formatearMoneda(baseImponibleAnual)}</span>
                    </div>
                    <div style="display: flex; justify-content: space-between; color: #333;">
                        <span>Impuesto Anual Calculado:</span>
                        <span>S/ ${formatearMoneda(impuestoRentaAnual)}</span>
                    </div>
                    <div style="display: flex; justify-content: space-between; font-weight: 600; color: #d32f2f; margin-top: 5px;">
                        <span>Descuento Mensual:</span>
                        <span>S/ ${formatearMoneda(impuestoRentaMensual)}</span>
                    </div>
                </div>
            </div>` : ''}

            <div style="margin-top: 30px; padding-top: 20px; border-top: 2px solid #e0e0e0;">
                <div style="display: flex; justify-content: space-between; margin-bottom: 10px; padding: 10px; background: #e3f2fd; border-radius: 5px;">
                    <span style="font-weight: 600; color: #1976d2;">Total Ingresos:</span>
                    <span style="font-weight: 600; color: #1976d2;">S/ ${formatearMoneda(totalIngresos)}</span>
                </div>
                <div style="display: flex; justify-content: space-between; margin-bottom: 10px; padding: 10px; background: #ffebee; border-radius: 5px;">
                    <span style="font-weight: 600; color: #d32f2f;">Total Descuentos:</span>
                    <span style="font-weight: 600; color: #d32f2f;">S/ ${formatearMoneda(totalDescuentos)}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 15px; background: #e8f5e9; border-radius: 5px;">
                    <span style="font-weight: 700; font-size: 16px; color: #2e7d32;">Neto a Pagar:</span>
                    <span style="font-weight: 700; font-size: 18px; color: #2e7d32;">S/ ${formatearMoneda(netoPagar)}</span>
                </div>
            </div>
        `;
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
        const nombres = emp.Nombres || emp.NombreCompleto || '';
        const apellidos = emp.Apellidos || '';

        if (nombres && apellidos) {
            return `${nombres} ${apellidos}`;
        }
        return nombres || apellidos || 'Sin nombre';
    }

    function formatearMoneda(valor) {
        if (valor == null || isNaN(valor)) return '0.00';
        return parseFloat(valor).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
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
        verDetalleEmpleado: verDetalleEmpleado,
        procesarNomina: procesarNomina,
        cerrarModales: cerrarModales
    };

})();

// AUTO-INICIALIZACIÓN
$(document).ready(function () {
    console.log('📄 DOM Ready - Iniciando NominaUI');

    setTimeout(() => {
        Alertas.info('Sistema de nómina cargado', 2000);
    }, 500);

    NominaUI.init();
});

window.NominaUI = NominaUI;
console.log('✅ Módulo NominaUI cargado correctamente');
