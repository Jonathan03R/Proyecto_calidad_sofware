// nomina.service.js
(function (global) {
    'use strict';

    const NominaService = {
        listarPeriodos: function () {
            return $.ajax({
                url: global.NominaConfig.urls.listarPeriodos,
                type: 'GET',
                dataType: 'json'
            });
        },

        obtenerEmpleadosVigentes: function (periodoId) {
            return $.ajax({
                url: global.NominaConfig.urls.obtenerEmpleadosVigentesPorPeriodo,
                type: 'GET',
                dataType: 'json',
                data: { periodoId: periodoId }
            });
        },

        // alias por compatibilidad (si antes llamabas procesarNomina)
        procesarNomina: function (periodoId) {
            return NominaService.iniciarProceso(periodoId);
        },

        // nuevo: crea cabecera de nómina y devuelve lista de pendientes
        iniciarProceso: function (periodoId) {
            return $.ajax({
                url: global.NominaConfig.urls.iniciarProceso,
                type: 'POST',
                dataType: 'json',
                data: { periodoId: periodoId }
            });
        },

        // nuevo: procesar 1 trabajador (llamado por el frontend por cada trabajador)
        procesarTrabajador: function (nominaId, trabajadorId, periodoId) {
            return $.ajax({
                url: global.NominaConfig.urls.procesarTrabajador,
                type: 'POST',
                dataType: 'json',
                data: {
                    nominaId: nominaId,
                    trabajadorId: trabajadorId,
                    periodoId: periodoId
                }
            });
        },

        // nuevo: cerrar proceso y actualizar periodo (huboErrores opcional)
        cerrarProceso: function (nominaId, periodoId, huboErrores) {
            return $.ajax({
                url: global.NominaConfig.urls.cerrarProceso,
                type: 'POST',
                dataType: 'json',
                data: {
                    nominaId: nominaId,
                    periodoId: periodoId,
                    huboErrores: huboErrores === true ? true : false
                }
            });
        },

        obtenerResumenProceso: function (periodoId) {
            return $.ajax({
                url: global.NominaConfig.urls.obtenerResumenProcesoNomina,
                type: 'GET',
                dataType: 'json',
                data: { periodoId: periodoId }
            });
        }
    };

    global.NominaService = NominaService;

})(window);
