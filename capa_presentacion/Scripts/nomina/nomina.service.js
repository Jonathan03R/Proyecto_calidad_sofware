let procesoInterval = null;
let procesoInicio = null;
let procesoTotalEmpleados = 0;

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
                data: { periodoId }
            });
        },

        procesarNomina: function (periodoId) {
            return $.ajax({
                url: global.NominaConfig.urls.procesarNomina,
                type: 'POST',
                dataType: 'json',
                data: { periodoId }
            });
        }
    };

    global.NominaService = NominaService;

})(window);
