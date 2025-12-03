(function () {
    'use strict';

    // ============================================
    // MÓDULO DE CONFIGURACIÓN Y CONSTANTES
    // ============================================

    const $page = $('#page-contratos');

    window.ContratoConfig = {
        // URLs de endpoints
        URLS: {
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
        },

        // Cache de datos
        Cache: {
            activos: [],
            sin: []
        }
    };

})();