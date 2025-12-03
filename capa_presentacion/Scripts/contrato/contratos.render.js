(function () {
    'use strict';

    // ============================================
    // MÓDULO DE RENDERIZADO DE TABLAS
    // ============================================

    window.ContratoRender = {

        /**
         * Renderiza la tabla de contratos activos
         */
        renderActivos: function (items) {
            const $tb = $('#tbody-contratos');
            const utils = window.ContratoUtils;

            if (!items.length) {
                $tb.html(utils.emptyRow(7, 'Sin contratos activos'));
                $('#txt-total').text('0');
                return;
            }

            const rows = items.map(it => `
                <tr>
                    <td><p class="empleado-nombre">${utils.esc(it.EmpleadoNombre)}</p></td>
                    <td>${utils.esc(it.Documento)}</td>
                    <td>${utils.esc(it.CargoNombre || '')}</td>
                    <td>${utils.esc(it.EstadoContratoNombre || '')}</td>
                    <td>${utils.fmtFecha(it.FechaInicio) || '-'}</td>
                    <td>${utils.fmtFecha(it.FechaFin) || '-'}</td>
                    <td>
                        <button
                            type="button"
                            class="btn btn-sm btn-secondary btn-editar-contrato"
                            data-contratoid="${it.ContratoId}"
                            data-trabid="${it.TrabajadorId}">
                            Editar
                        </button>
                    </td>
                </tr>`).join('');

            $tb.html(rows);
            $('#txt-total').text(`${items.length} registro(s)`);
        },

        /**
         * Renderiza la tabla de empleados sin contrato
         */
        renderSin: function (items) {
            const $tb = $('#tbody-sin-contrato');
            const utils = window.ContratoUtils;

            if (!items.length) {
                $tb.html(utils.emptyRow(3, 'No hay empleados sin contrato'));
                $('#txt-total-sin').text('0');
                return;
            }

            const rows = items.map(it => `
                <tr>
                    <td>${utils.esc(it.EmpleadoNombre)}</td>
                    <td>${utils.esc(it.Documento)}</td>
                    <td>
                        <button class="btn btn-primary" data-trabid="${it.TrabajadorId}">
                            Nuevo Contrato
                        </button>
                    </td>
                </tr>`).join('');

            $tb.html(rows);
            $('#txt-total-sin').text(`${items.length} sin contrato`);
        },

        /**
         * Renderiza el resumen de contratos
         */
        renderResumen: function (data) {
            $('#cr_total_contratos').text(data.TotalContratos);
            $('#cr_contratos_activos').text(data.ContratosActivos);
            $('#cr_por_vencer_30').text(data.PorVencer30);
            $('#cr_alertas_legales').text(data.AlertasLegales);
        }

    };

})();