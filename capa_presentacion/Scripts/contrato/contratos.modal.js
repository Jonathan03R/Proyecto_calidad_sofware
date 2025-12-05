(function () {
    'use strict';


    globalThis.ContratoModal = {


        open: function (id) {
            const $m = $('#' + id);
            $m.attr('aria-hidden', 'false').addClass('is-open');
            $('body').addClass('modal-open');
        },


        close: function (id) {
            const $m = $('#' + id);
            $m.attr('aria-hidden', 'true').removeClass('is-open');
            $('body').removeClass('modal-open');
        },


        init: function () {
            
            $(document).on('click', '[data-modal-close]', function () {
                ContratoModal.close($(this).data('modal-close'));
            });

            
            $(document).on('click', '#modal-nuevo-contrato .modal-backdrop', function () {
                ContratoModal.close('modal-nuevo-contrato');
            });

            
            $(document).on('keydown', function (e) {
                if (e.key === 'Escape') {
                    ContratoModal.close('modal-nuevo-contrato');
                    ContratoModal.close('modal-editar-contrato');
                }
            });
        }

    };

})();