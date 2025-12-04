(function () {
    'use strict';

    // ============================================
    // MÓDULO DE GESTIÓN DE FORMULARIOS
    // ============================================

    window.ContratoForm = {

        /**
         * Abre el formulario de nuevo contrato
         */
        abrirNuevoContrato: function (trabajadorId) {
            const config = window.ContratoConfig;
            const item = (config.Cache.sin || []).find(x => Number(x.TrabajadorId) === trabajadorId);

            if (!item) return;

            // Llenar datos del trabajador
            $('#nc_trabajador_id').val(trabajadorId);
            $('#nc_nombre').val(item.EmpleadoNombre || '');
            $('#nc_dni').val(item.Documento || '');

            // Limpiar campos del contrato
            this.limpiarFormularioNuevo();

            // Cargar catálogos
            window.ContratoCatalogs.cargarTodosNuevo();

            // Abrir modal
            window.ContratoModal.open('modal-nuevo-contrato');
            setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
        },

        /**
         * Abre el formulario de edición de contrato
         */
        abrirEdicionContrato: function (contratoId) {
            const config = window.ContratoConfig;
            const item = (config.Cache.activos || []).find(x => Number(x.ContratoId) === contratoId);

            if (!item) return;

            // Llenar datos básicos
            $('#ec_contrato_id').val(contratoId);
            $('#ec_trabajador_id').val(item.TrabajadorId || '');
            $('#ec_empleado').val(item.EmpleadoNombre || '');
            $('#ec_motivo').val('');
            $('#ec_mensaje').text('');

            // Llenar campos del contrato
            $('#ec_salario').val(item.Salario || '');
            $('#ec_modo_pago').val(item.ModoPago || '');
            $('#ec_horas_semanales').val(item.HorasSemanales || '');
            $('#ec_fecha_inicio').val(item.FechaInicio || '');
            $('#ec_fecha_fin').val(item.FechaFin || '');
            $('#ec_tarifa_hora').val(item.TarifaHora || '');
            $('#ec_descripcion_funciones').val(item.DescripcionFunciones || '');
            $('#ec_observaciones').val(item.Observaciones || '');

            // Cargar catálogos con valores seleccionados
            window.ContratoCatalogs.cargarTodosEdicion(item);

            // Abrir modal
            window.ContratoModal.open('modal-editar-contrato');
        },

        /**
         * Limpia el formulario de nuevo contrato
         */
        limpiarFormularioNuevo: function () {
            $('#nc_cargo_id').val('');
            $('#nc_area_id').val('');
            $('#nc_tipo_pension_id').val('');
            $('#nc_tipo_salario_id').val('');
            $('#nc_tipo_jornada_id').val('');
            $('#nc_modo_pago').val('');
            $('#nc_fecha_inicio').val('');
            $('#nc_fecha_fin').val('');
            $('#nc_remuneracion').val('');
            $('#nc_tarifa_hora').val('');
            $('#nc_descripcion_funciones').val('');
            $('#nc_observaciones').val('');
            $('#nc_mensaje').text('');
            $('#nc_horas_semanales').val('48');
        },

        /**
         * Obtiene los datos del formulario de nuevo contrato
         */
        obtenerDatosNuevo: function () {
            return {
                TrabajadorId: Number($('#nc_trabajador_id').val()),
                CargoId: $('#nc_cargo_id').val() ? Number($('#nc_cargo_id').val()) : null,
                AreaId: $('#nc_area_id').val() ? Number($('#nc_area_id').val()) : null,
                TipoPensionId: $('#nc_tipo_pension_id').val() ? Number($('#nc_tipo_pension_id').val()) : null,
                TipoSalarioId: $('#nc_tipo_salario_id').val() ? Number($('#nc_tipo_salario_id').val()) : null,
                TipoJornadaId: $('#nc_tipo_jornada_id').val() ? Number($('#nc_tipo_jornada_id').val()) : null,
                FechaInicio: $('#nc_fecha_inicio').val(),
                FechaFin: $('#nc_fecha_fin').val() || null,
                Salario: $('#nc_remuneracion').val() ? parseFloat($('#nc_remuneracion').val()) : null,
                HorasSemanales: $('#nc_horas_semanales').val() ? parseInt($('#nc_horas_semanales').val(), 10) : null,
                TarifaHora: $('#nc_tarifa_hora').val() ? parseFloat($('#nc_tarifa_hora').val()) : null,
                ModoPago: $('#nc_modo_pago').val() || null,
                DescripcionFunciones: $('#nc_descripcion_funciones').val() || null,
                Observaciones: $('#nc_observaciones').val() || null
            };
        },

        /**
         * Obtiene los datos del formulario de edición
         */
        obtenerDatosEdicion: function () {
            return {
                ContratoId: Number($('#ec_contrato_id').val()),
                TrabajadorId: Number($('#ec_trabajador_id').val()),
                Motivo: $('#ec_motivo').val().trim(),
                CargoId: $('#ec_cargo_id').val() ? Number($('#ec_cargo_id').val()) : null,
                AreaId: $('#ec_area_id').val() ? Number($('#ec_area_id').val()) : null,
                TipoPensionId: $('#ec_tipo_pension_id').val() ? Number($('#ec_tipo_pension_id').val()) : null,
                TipoSalarioId: $('#ec_tipo_salario_id').val() ? Number($('#ec_tipo_salario_id').val()) : null,
                TipoJornadaId: $('#ec_tipo_jornada_id').val() ? Number($('#ec_tipo_jornada_id').val()) : null,
                FechaInicio: $('#ec_fecha_inicio').val(),
                FechaFin: $('#ec_fecha_fin').val() || null,
                HorasSemanales: $('#ec_horas_semanales').val() ? parseInt($('#ec_horas_semanales').val(), 10) : null,
                Salario: $('#ec_salario').val() ? parseFloat($('#ec_salario').val()) : null,
                TarifaHora: $('#ec_tarifa_hora').val() ? parseFloat($('#ec_tarifa_hora').val()) : null,
                ModoPago: $('#ec_modo_pago').val() || null,
                DescripcionFunciones: $('#ec_descripcion_funciones').val() || null,
                Observaciones: $('#ec_observaciones').val() || null
            };
        },

        /**
         * Recalcula la tarifa por hora en el formulario de nuevo contrato
         */
        recalcularTarifaHoraNuevo: function () {
            const salario = parseFloat($('#nc_remuneracion').val());
            const horas = parseInt($('#nc_horas_semanales').val(), 10);
            const tarifa = window.ContratoUtils.calcularTarifaHora(salario, horas);

            $('#nc_tarifa_hora').val(tarifa || '');
        }

    };

})();