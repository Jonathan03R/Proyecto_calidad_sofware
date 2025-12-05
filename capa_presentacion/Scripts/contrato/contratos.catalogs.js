(function () {
    'use strict';

    // ============================================
    // MÓDULO DE CARGA DE CATÁLOGOS
    // ============================================

    window.ContratoCatalogs = {

        /**
         * Carga áreas en un select
         */
        cargarAreas: function (selector, selectedId) {
            const $select = $(selector || '#nc_area_id');
            $select.empty().append('<option value="">Seleccione área</option>');

            $.getJSON(window.ContratoConfig.URLS.obtenerAreas)
                .done(function (data) {
                    (data || []).forEach(function (area) {
                        $select.append(
                            $('<option>', {
                                value: area.id,
                                text: area.nombre
                            })
                        );
                    });

                    if (selectedId != null) {
                        $select.val(String(selectedId));
                    }
                })
                .fail(function () {
                    console.error('Error al cargar áreas');
                });
        },

        /**
         * Carga cargos en un select
         */
        cargarCargos: function (selector, selectedId) {
            const $select = $(selector || '#nc_cargo_id');
            $select.empty().append('<option value="">Seleccione cargo</option>');

            $.getJSON(window.ContratoConfig.URLS.obtenerCargos)
                .done(function (data) {
                    (data || []).forEach(function (cargo) {
                        $select.append(
                            $('<option>', {
                                value: cargo.id,
                                text: cargo.nombre
                            })
                        );
                    });

                    if (selectedId != null) {
                        $select.val(String(selectedId));
                    }
                })
                .fail(function () {
                    console.error('Error al cargar cargos');
                });
        },

        /**
         * Carga pensiones en un select
         */
        cargarPensiones: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_pension_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(window.ContratoConfig.URLS.obtenerPensiones)
                .done(function (data) {
                    (data || []).forEach(function (p) {
                        const texto = p.entidad
                            ? `${p.nombre} (${p.entidad})`
                            : p.nombre;

                        $select.append(
                            $('<option>', {
                                value: p.id,
                                text: texto
                            })
                        );
                    });

                    if (selectedId != null) {
                        $select.val(String(selectedId));
                    }
                })
                .fail(function () {
                    console.error('Error al cargar pensiones');
                });
        },

        /**
         * Carga tipos de salario en un select
         */
        cargarTiposSalarios: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_salario_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(window.ContratoConfig.URLS.obtenerTiposSalarios)
                .done(function (data) {
                    (data || []).forEach(function (t) {
                        $select.append(
                            $('<option>', {
                                value: t.id,
                                text: t.nombre
                            })
                        );
                    });

                    if (selectedId != null) {
                        $select.val(String(selectedId));
                    }
                })
                .fail(function () {
                    console.error('Error al cargar tipos de salario');
                });
        },

        /**
         * Carga jornadas en un select
         */
        cargarJornadas: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_jornada_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(window.ContratoConfig.URLS.obtenerJornadas)
                .done(function (data) {
                    (data || []).forEach(function (j) {
                        $select.append(
                            $('<option>', {
                                value: j.id,
                                text: j.nombre
                            })
                        );
                    });

                    // Para NUEVO contrato → NO seleccionar nada por defecto
                    if (selector === undefined || selector === '#nc_tipo_jornada_id') {
                        if (selectedId != null) {
                            $select.val(String(selectedId));
                        }
                    }
                })
                .fail(function () {
                    console.error('Error al cargar tipos de jornada');
                });
        },

        /**
         * Carga todos los catálogos para nuevo contrato
         */
        cargarTodosNuevo: function () {
            this.cargarAreas();
            this.cargarCargos();
            this.cargarPensiones();
            //this.cargarTiposSalarios();
            //this.cargarJornadas();
        },

        /**
         * Carga todos los catálogos para edición de contrato
         */
        cargarTodosEdicion: function (item) {
            this.cargarAreas('#ec_area_id', item.AreaId);
            this.cargarPensiones('#ec_tipo_pension_id', item.TipoPensionId);
            this.cargarCargos('#ec_cargo_id', item.CargoId);
            /*this.cargarTiposSalarios('#ec_tipo_salario_id', item.TipoSalarioId);*/
            //this.cargarJornadas('#ec_tipo_jornada_id', item.TipoJornadaId);
        }

    };

})();