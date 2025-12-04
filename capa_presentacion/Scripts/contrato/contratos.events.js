(function () {
    'use strict';

    // ============================================
    // MÓDULO DE EVENTOS Y CONTROLADORES
    // ============================================

    window.ContratoEvents = {

        /**
         * Inicializa todos los eventos de la página
         */
        init: function () {
            this.initTabs();
            this.initFiltros();
            this.initBotones();
            this.initFormularios();
        },

        /**
         * Inicializa eventos de pestañas
         */
        initTabs: function () {
            $(document).on('click', '.tab-btn', function () {
                const tab = $(this).data('tab');

                $('.tab-btn').removeClass('is-active');
                $(this).addClass('is-active');

                $('.tab-panel').removeClass('is-active');
                $('#tab-' + tab).addClass('is-active');

                if (tab === 'activos') {
                    window.ContratoData.cargarActivos();
                } else {
                    window.ContratoData.cargarSin();
                }
            });
        },

        /**
         * Inicializa eventos de filtros
         */
        initFiltros: function () {
            // Botón filtrar
            $(document).on('click', '#fc_filtrar', function () {
                window.ContratoData.aplicarFiltro();
            });

            // Enter en campo de búsqueda
            $(document).on('keydown', '#fc_query', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    window.ContratoData.aplicarFiltro();
                }
            });

            // Input con debounce
            let t;
            $(document).on('input', '#fc_query', function () {
                clearTimeout(t);
                t = setTimeout(function () {
                    window.ContratoData.aplicarFiltro();
                }, 150);
            });

            // Reset del formulario
            $(document).on('reset', '#form-filtros-contratos', function () {
                setTimeout(function () {
                    window.ContratoData.aplicarFiltro();
                }, 0);
            });
        },

        /**
         * Inicializa eventos de botones
         */
        initBotones: function () {
            // Botón nuevo contrato (desde sin contrato)
            $(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
                const id = Number($(this).data('trabid'));
                window.ContratoForm.abrirNuevoContrato(id);
            });

            // Botón editar contrato
            $(document).on('click', '.btn-editar-contrato', function () {
                const contratoId = Number($(this).data('contratoid'));
                window.ContratoForm.abrirEdicionContrato(contratoId);
            });
        },

        /**
         * Inicializa eventos de formularios
         */
        initFormularios: function () {
            // Confirmar creación de contrato
            $(document).on('click', '#nc_confirmar', function () {
                const contrato = window.ContratoForm.obtenerDatosNuevo();

                // Validar
                const validacion = window.ContratoValidation.validarNuevoContrato(contrato);

                if (!validacion.valido) {
                    $('#nc_mensaje').text(validacion.mensaje);
                    return;
                }

                // Guardar
                window.ContratoAPI.crearContrato(contrato);
            });

            // Confirmar edición de contrato
            $(document).on('click', '#ec_confirmar', function () {
                const data = window.ContratoForm.obtenerDatosEdicion();

                // Validar
                const validacion = window.ContratoValidation.validarEdicionContrato(data, data.Motivo);

                if (!validacion.valido) {
                    $('#ec_mensaje').text(validacion.mensaje);
                    return;
                }

                // Actualizar
                window.ContratoAPI.actualizarContrato(data);
            });

            // Recalcular tarifa hora
            $(document).on('input', '#nc_remuneracion, #nc_horas_semanales', function () {
                window.ContratoForm.recalcularTarifaHoraNuevo();
            });
        }

    };

})();