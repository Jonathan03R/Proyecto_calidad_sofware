(function () {
    'use strict';

    // ============================================
    // MÓDULO DE CARGA DE DATOS
    // ============================================

    globalThis.ContratoData = {

        cargarActivos: function () {
            const $tb = $('#tbody-contratos');
            const utils = globalThis.ContratoUtils;
            const config = globalThis.ContratoConfig;

            $tb.html(utils.loadingRow(7));
            $('#txt-total').text('');
            $('#paginador').empty();

            $.get(config.URLS.listarActivos, function (resp) {
                if (!resp?.consultaExitosa) {
                    $tb.html(utils.emptyRow(7, resp?.mensaje || 'No se pudieron obtener los contratos activos'));
                    return;
                }
                config.Cache.activos = resp.data || [];
                ContratoData.aplicarFiltro();
            }).fail(function () {
                $tb.html(utils.emptyRow(7, 'Error de conexión'));
            });
        },

        cargarSin: function () {
            const $tb = $('#tbody-sin-contrato');
            const utils = globalThis.ContratoUtils;
            const config = globalThis.ContratoConfig;

            $tb.html(utils.loadingRow(3));

            $.get(config.URLS.listarSin, function (resp) {
                if (!resp?.consultaExitosa) {
                    $tb.html(utils.emptyRow(3, resp?.mensaje || 'No se pudo obtener la lista'));
                    return;
                }
                config.Cache.sin = resp.data || [];
                ContratoData.aplicarFiltro();
            }).fail(function () {
                $tb.html(utils.emptyRow(3, 'Error de conexión'));
            });
        },

        cargarResumen: function () {
            const config = globalThis.ContratoConfig;

            $.get(config.URLS.resumenContratos, function (resp) {
                if (!resp?.exito || !resp.data) {
                    console.error('No se pudo obtener el resumen de contratos:', resp?.mensaje);
                    return;
                }

                globalThis.ContratoRender.renderResumen(resp.data);
            }).fail(function () {
                console.error('Error de conexión al obtener el resumen de contratos');
            });
        },

        aplicarFiltro: function () {
            const utils = globalThis.ContratoUtils;
            const config = globalThis.ContratoConfig;
            const qn = utils.norm($('#fc_query').val());

            if (utils.tabActiva() === 'activos') {
                const filtered = (config.Cache.activos || []).filter(x => utils.coincide(x, qn));
                globalThis.ContratoRender.renderActivos(filtered);
            } else {
                const filtered = (config.Cache.sin || []).filter(x => utils.coincide(x, qn));
                globalThis.ContratoRender.renderSin(filtered);
            }
        }

    };

})();