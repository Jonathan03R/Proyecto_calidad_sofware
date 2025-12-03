// nomina.alertas.js (implementación mínima, sin DOM)
(function (global) {
    'use strict';

    const Alertas = (function () {
        // controla si quieres ver logs en consola
        const DEBUG = false;

        function log(type, msg) {
            if (!DEBUG) return;
            switch (type) {
                case 'error': console.error('[ALERTA]', msg); break;
                case 'validacion': console.warn('[ALERTA]', msg); break;
                default: console.log('[ALERTA]', msg); break;
            }
        }

        return {
            exito: (msg) => log('info', msg),
            error: (msg) => log('error', msg),
            info: (msg) => log('info', msg),
            validacion: (msg) => log('validacion', msg),
            cargando: (msg) => log('info', msg),
            ocultarTodas: () => { }
        };
    })();

    global.Alertas = Alertas;
})(window);
