(function () {
    'use strict';

    // ============================================
    // MÓDULO DE GESTIÓN DE MODALES
    // ============================================

    window.ContratoModal = {

        /**
         * Abre un modal por su ID
         */
        open: function (id) {
            const $m = $('#' + id);
            $m.attr('aria-hidden', 'false').addClass('is-open');
            $('body').addClass('modal-open');
        },

        /**
         * Cierra un modal por su ID
         */
        close: function (id) {
            const $m = $('#' + id);
            $m.attr('aria-hidden', 'true').removeClass('is-open');
            $('body').removeClass('modal-open');
        },

        /**
         * Inicializa los eventos de los modales
         */
        init: function () {
            // Botón de cerrar modal
            $(document).on('click', '[data-modal-close]', function () {
                ContratoModal.close($(this).data('modal-close'));
            });

            // Click en backdrop
            $(document).on('click', '#modal-nuevo-contrato .modal-backdrop', function () {
                ContratoModal.close('modal-nuevo-contrato');
            });

            // Tecla Escape
            $(document).on('keydown', function (e) {
                if (e.key === 'Escape') {
                    ContratoModal.close('modal-nuevo-contrato');
                    ContratoModal.close('modal-editar-contrato');
                }
            });
        }

    };

})();