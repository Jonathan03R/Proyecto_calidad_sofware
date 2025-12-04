// nomina.kpis.js
(function (global) {
    'use strict';

    function init(elements) {
        if (!elements) return;
        elements.kpiPendientes.text('—');
        elements.kpiProcesadas.text('—');
        elements.kpiInactivos.text('—');
        elements.kpiTotalNomina.text('—');
    }

    function desdeEmpleados(empleados, elements, formatearMoneda) {
        if (!elements) return;

        if (!empleados || empleados.length === 0) {
            elements.kpiPendientes.text('0');
            elements.kpiProcesadas.text('0');
            elements.kpiInactivos.text('0');
            elements.kpiTotalNomina.text('0.00');
            return;
        }

        const total = empleados.length;
        const inactivos = empleados.filter(e => (e.Estado || '').toUpperCase() !== 'ACTIVO').length;
        const totalNeto = empleados.reduce((sum, e) => sum + (e.NetoPagar || 0), 0);

        elements.kpiPendientes.text('0'); // pendientes luego si cambias lógica
        elements.kpiProcesadas.text(total);
        elements.kpiInactivos.text(inactivos);
        elements.kpiTotalNomina.text('S/ ' + formatearMoneda(totalNeto));
    }

    // ✅ NUEVO: KPIs que vienen del backend
    function desdeResumen(kpis, elements, formatearMoneda) {
        if (!elements || !kpis) return;

        elements.kpiPendientes.text(kpis.totalPeriodosAbiertos || 0);
        elements.kpiProcesadas.text(kpis.totalPeriodosProcesados || 0);
        elements.kpiInactivos.text(kpis.totalTrabajadoresInactivos || 0);

        const totalNeto = kpis.totalNetoGeneral || 0;
        elements.kpiTotalNomina.text('S/ ' + formatearMoneda(totalNeto));
    }

    global.NominaKPIs = {
        init,
        desdeEmpleados,
        desdeResumen // 👈 IMPORTANTE: ahora también exportamos esta
    };

    console.log('✅ nomina.kpis.js cargado');

})(window);
