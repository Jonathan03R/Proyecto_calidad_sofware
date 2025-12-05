(function () {
    'use strict';

    // ============================================
    // MÓDULO DE CARGA DE CATÁLOGOS
    // ============================================

    globalThis.ContratoCatalogs = {

        cargarAreas: function (selector, selectedId) {
            const $select = $(selector || '#nc_area_id');
            $select.empty().append('<option value="">Seleccione área</option>');

            $.getJSON(globalThis.ContratoConfig.URLS.obtenerAreas)
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

            $.getJSON(globalThis.ContratoConfig.URLS.obtenerCargos)
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

        cargarPensiones: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_pension_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(globalthis.ContratoConfig.URLS.obtenerPensiones)
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

        cargarTiposSalarios: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_salario_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(globalThis.ContratoConfig.URLS.obtenerTiposSalarios)
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

        cargarJornadas: function (selector, selectedId) {
            const $select = $(selector || '#nc_tipo_jornada_id');
            $select.empty().append('<option value="">Seleccione</option>');

            $.getJSON(globalThis.ContratoConfig.URLS.obtenerJornadas)
                .done(function (data) {
                    (data || []).forEach(function (j) {
                        $select.append(
                            $('<option>', {
                                value: j.id,
                                text: j.nombre
                            })
                        );
                    });

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

        cargarTodosNuevo: function () {
            this.cargarAreas();
            this.cargarCargos();
            this.cargarPensiones();
        },

        cargarTodosEdicion: function (item) {
            this.cargarAreas('#ec_area_id', item.AreaId);
            this.cargarPensiones('#ec_tipo_pension_id', item.TipoPensionId);
            this.cargarCargos('#ec_cargo_id', item.CargoId);
            this.cargarJornadas('#ec_tipo_jornada_id', item.TipoJornadaId);

        }

    };

})();