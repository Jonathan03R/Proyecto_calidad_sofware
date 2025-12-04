(function () {
    'use strict';

    // ============================================
    // MÓDULO DE CARGA DE DATOS
    // ============================================

    window.ContratoData = {

        /**
         * Carga la lista de contratos activos
         */
        cargarActivos: function () {
            const $tb = $('#tbody-contratos');
            const utils = window.ContratoUtils;
            const config = window.ContratoConfig;

            $tb.html(utils.loadingRow(7));
            $('#txt-total').text('');
            $('#paginador').empty();

            $.get(config.URLS.listarActivos, function (resp) {
                if (!resp || !resp.consultaExitosa) {
                    $tb.html(utils.emptyRow(7, resp?.mensaje || 'No se pudieron obtener los contratos activos'));
                    return;
                }
                config.Cache.activos = resp.data || [];
                ContratoData.aplicarFiltro();
            }).fail(function () {
                $tb.html(utils.emptyRow(7, 'Error de conexión'));
            });
        },

        /**
         * Carga la lista de empleados sin contrato
         */
        cargarSin: function () {
            const $tb = $('#tbody-sin-contrato');
            const utils = window.ContratoUtils;
            const config = window.ContratoConfig;

            $tb.html(utils.loadingRow(3));

            $.get(config.URLS.listarSin, function (resp) {
                if (!resp || !resp.consultaExitosa) {
                    $tb.html(utils.emptyRow(3, resp?.mensaje || 'No se pudo obtener la lista'));
                    return;
                }
                config.Cache.sin = resp.data || [];
                ContratoData.aplicarFiltro();
            }).fail(function () {
                $tb.html(utils.emptyRow(3, 'Error de conexión'));
            });
        },

        /**
         * Carga el resumen de contratos
         */
        cargarResumen: function () {
            const config = window.ContratoConfig;

            $.get(config.URLS.resumenContratos, function (resp) {
                if (!resp || !resp.exito || !resp.data) {
                    console.error('No se pudo obtener el resumen de contratos:', resp && resp.mensaje);
                    return;
                }

                window.ContratoRender.renderResumen(resp.data);
            }).fail(function () {
                console.error('Error de conexión al obtener el resumen de contratos');
            });
        },

        /**
         * Aplica el filtro de búsqueda a los datos actuales
         */
        aplicarFiltro: function () {
            const utils = window.ContratoUtils;
            const config = window.ContratoConfig;
            const qn = utils.norm($('#fc_query').val());

            if (utils.tabActiva() === 'activos') {
                const filtered = (config.Cache.activos || []).filter(x => utils.coincide(x, qn));
                window.ContratoRender.renderActivos(filtered);
            } else {
                const filtered = (config.Cache.sin || []).filter(x => utils.coincide(x, qn));
                window.ContratoRender.renderSin(filtered);
            }
        }

    };

})();