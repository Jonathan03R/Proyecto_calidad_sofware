(function () {
    'use strict';

    // ============================================
    // MÓDULO DE LLAMADAS A LA API
    // ============================================

    globalThis.ContratoAPI = {

        /**
         * Crea un nuevo contrato
         */
        crearContrato: function (contrato, callbacks) {
            const config = globalThis.ContratoConfig;
            callbacks = callbacks || {};

            $('#nc_mensaje').text('Guardando contrato...');

            $.ajax({
                url: config.URLS.crearContrato,
                type: 'POST',
                data: JSON.stringify(contrato),
                contentType: 'application/json; charset=utf-8',
                success: function (resp) {
                    if (resp?.exito) {
                        $('#nc_mensaje').text('Contrato creado correctamente.');

                        setTimeout(function () {
                            $('#nc_mensaje').text('');

                            // Recargar datos
                            if (globalThis.ContratosUI) {
                                globalThis.ContratosUI.recargarActivos();
                                globalThis.ContratosUI?.recargarSin?.();
                                globalThis.ContratosUI?.recargarResumen?.();
                            }

                            // Cerrar modal
                            $('[data-modal-close="modal-nuevo-contrato"]').click();

                            // Callback de éxito
                            if (callbacks.onSuccess) callbacks.onSuccess(resp);
                        }, 700);
                    } else {
                        const mensaje = resp?.mensaje ? resp.mensaje : 'No se pudo crear el contrato.';
                        $('#nc_mensaje').text(mensaje);

                        if (callbacks.onError) callbacks.onError(mensaje);
                    }
                },
                error: function () {
                    const mensaje = 'Error de conexión al crear el contrato.';
                    $('#nc_mensaje').text(mensaje);

                    if (callbacks.onError) callbacks.onError(mensaje);
                }
            });
        },

        /**
         * Actualiza un contrato existente
         */
        actualizarContrato: function (data, callbacks) {
            const config = globalThis.ContratoConfig;
            callbacks = callbacks || {};

            $('#ec_mensaje').text('Guardando cambios...');

            $.ajax({
                url: config.URLS.actualizarContrato,
                type: 'POST',
                data: data,
                success: function (resp) {
                    if (resp?.exito) {
                        $('#ec_mensaje').text('Contrato actualizado correctamente.');

                        // Recargar datos
                        if (globalThis.ContratosUI) {
                            globalThis.ContratosUI.recargarActivos();
                        }

                        setTimeout(function () {
                            $('#ec_mensaje').text('');

                            // Cerrar modal
                            $('[data-modal-close="modal-editar-contrato"]').click();

                            // Callback de éxito
                            if (callbacks.onSuccess) callbacks.onSuccess(resp);
                        }, 700);
                    } else {
                        const mensaje = resp?.mensaje
                            ? resp.mensaje
                            : 'No se pudo actualizar el contrato.';
                        $('#ec_mensaje').text(mensaje);

                        if (callbacks.onError) callbacks.onError(mensaje);
                    }
                },
                error: function () {
                    const mensaje = 'Error de conexión al actualizar el contrato.';
                    $('#ec_mensaje').text(mensaje);

                    if (callbacks.onError) callbacks.onError(mensaje);
                }
            });
        }

    };

})();