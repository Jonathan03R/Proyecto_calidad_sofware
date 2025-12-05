(function () {
    'use strict';



    globalThis.ContratoEvents = {

        init: function () {
            this.initTabs();
            this.initFiltros();
            this.initBotones();
            this.initFormularios();
        },


        initTabs: function () {
            $(document).on('click', '.tab-btn', function () {
                const tab = $(this).data('tab');

                $('.tab-btn').removeClass('is-active');
                $(this).addClass('is-active');

                $('.tab-panel').removeClass('is-active');
                $('#tab-' + tab).addClass('is-active');

                if (tab === 'activos') {
                    globalThis.ContratoData.cargarActivos();
                } else {
                    globalThis.ContratoData.cargarSin();
                }
            });
        },


        initFiltros: function () {
            
            $(document).on('click', '#fc_filtrar', function () {
                globalThis.ContratoData.aplicarFiltro();
            });

            
            $(document).on('keydown', '#fc_query', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    globalThis.ContratoData.aplicarFiltro();
                }
            });

           
            let t;
            $(document).on('input', '#fc_query', function () {
                clearTimeout(t);
                t = setTimeout(function () {
                    globalThis.ContratoData.aplicarFiltro();
                }, 150);
            });

           
            $(document).on('reset', '#form-filtros-contratos', function () {
                setTimeout(function () {
                    globalThis.ContratoData.aplicarFiltro();
                }, 0);
            });
        },


        initBotones: function () {
           
            $(document).on('click', '#tbody-sin-contrato [data-trabid]', function () {
                const id = Number($(this).data('trabid'));
                globalThis.ContratoForm.abrirNuevoContrato(id);
            });

           
            $(document).on('click', '.btn-editar-contrato', function () {
                const contratoId = Number($(this).data('contratoid'));
                globalThis.ContratoForm.abrirEdicionContrato(contratoId);
            });
        },


        initFormularios: function () {
            
            $(document).on('click', '#nc_confirmar', function () {
                const contrato = globalThis.ContratoForm.obtenerDatosNuevo();

               
                const validacion = globalThis.ContratoValidation.validarNuevoContrato(contrato);

                if (!validacion.valido) {
                    $('#nc_mensaje').text(validacion.mensaje);
                    return;
                }

               
                globalThis.ContratoAPI.crearContrato(contrato);
            });

            
            $(document).on('click', '#ec_confirmar', function () {
                const data = globalThis.ContratoForm.obtenerDatosEdicion();

                
                const validacion = globalThis.ContratoValidation.validarEdicionContrato(data, data.Motivo);

                if (!validacion.valido) {
                    $('#ec_mensaje').text(validacion.mensaje);
                    return;
                }

                
                globalThis.ContratoAPI.actualizarContrato(data);
            });

            
            $(document).on('input', '#nc_remuneracion, #nc_horas_semanales', function () {
                globalThis.ContratoForm.recalcularTarifaHoraNuevo();
            });
        }

    };

})();