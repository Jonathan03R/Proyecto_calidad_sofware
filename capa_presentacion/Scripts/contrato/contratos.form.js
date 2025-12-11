(function () {
    'use strict';



    globalThis.ContratoForm = {


        abrirNuevoContrato: function (trabajadorId) {
            const config = globalThis.ContratoConfig;
            const item = (config.Cache.sin || []).find(x => Number(x.TrabajadorId) === trabajadorId);

            if (!item) return;

           
            $('#nc_trabajador_id').val(trabajadorId);
            $('#nc_nombre').val(item.EmpleadoNombre || '');
            $('#nc_dni').val(item.Documento || '');

            
            this.limpiarFormularioNuevo();

            
            globalThis.ContratoCatalogs.cargarTodosNuevo();


            globalThis.ContratoModal.open('modal-nuevo-contrato');
            setTimeout(() => $('#nc_cargo_id').trigger('focus'), 50);
        },


        abrirEdicionContrato: function (contratoId) {
            const config = globalThis.ContratoConfig;
            const item = (config.Cache.activos || []).find(x => Number(x.ContratoId) === contratoId);

            if (!item) return;

            
            $('#ec_contrato_id').val(contratoId);
            $('#ec_trabajador_id').val(item.TrabajadorId || '');
            $('#ec_empleado').val(item.EmpleadoNombre || '');
            $('#ec_motivo').val('');
            $('#ec_mensaje').text('');


            $('#ec_salario').val(item.Salario || '');
            $('#ec_modo_pago').val('Depósito');
            $('#ec_modo_pago_id').val('1');
            $('#ec_horas_semanales').val(item.HorasSemanales || 48).prop('readonly', true);
            $('#ec_fecha_inicio').val(item.FechaInicio || '');
            $('#ec_fecha_fin').val(item.FechaFin || '');
            $('#ec_tarifa_hora').val(item.TarifaHora || '');
            $('#ec_descripcion_funciones').val(item.DescripcionFunciones || '');
            $('#ec_observaciones').val(item.Observaciones || '');

            // Cargar catálogos con valores seleccionados
            globalThis.ContratoCatalogs.cargarTodosEdicion(item);

            // Abrir modal
            globalThis.ContratoModal.open('modal-editar-contrato');
        },

      
        limpiarFormularioNuevo: function () {
            $('#nc_cargo_id').val('');
            $('#nc_area_id').val('');
            $('#nc_tipo_pension_id').val('');
            $('#nc_modo_pago').val('Depósito');
            $('#nc_modo_pago_id').val('1');
            $('#nc_fecha_inicio').val('');
            $('#nc_fecha_fin').val('');
            $('#nc_remuneracion').val('');
            $('#nc_tarifa_hora').val('');
            $('#nc_descripcion_funciones').val('');
            $('#nc_observaciones').val('');
            $('#nc_mensaje').text('');
            $('#nc_horas_semanales').val('48');
        },


        obtenerDatosNuevo: function () {
            return {
                TrabajadorId: Number($('#nc_trabajador_id').val()),
                CargoId: $('#nc_cargo_id').val() ? Number($('#nc_cargo_id').val()) : null,
                AreaId: $('#nc_area_id').val() ? Number($('#nc_area_id').val()) : null,
                TipoPensionId: $('#nc_tipo_pension_id').val() ? Number($('#nc_tipo_pension_id').val()) : null,
                TipoSalarioId: 1,
                TipoJornadaId: 1,
                FechaInicio: $('#nc_fecha_inicio').val(),
                FechaFin: $('#nc_fecha_fin').val() || null,
                Salario: $('#nc_remuneracion').val() ? Number.parseFloat($('#nc_remuneracion').val()) : null,
                HorasSemanales: $('#nc_horas_semanales').val() ? Number.parseInt($('#nc_horas_semanales').val(), 10) : null,
                TarifaHora: $('#nc_tarifa_hora').val() ? Number.parseFloat($('#nc_tarifa_hora').val()) : null,
                ModoPagoId: 1,
                DescripcionFunciones: $('#nc_descripcion_funciones').val() || null,
                Observaciones: $('#nc_observaciones').val() || null
            };
        },


        obtenerDatosEdicion: function () {
            return {
                ContratoId: Number($('#ec_contrato_id').val()),
                TrabajadorId: Number($('#ec_trabajador_id').val()),
                Motivo: $('#ec_motivo').val().trim(),
                CargoId: $('#ec_cargo_id').val() ? Number($('#ec_cargo_id').val()) : null,
                AreaId: $('#ec_area_id').val() ? Number($('#ec_area_id').val()) : null,
                TipoPensionId: $('#ec_tipo_pension_id').val() ? Number($('#ec_tipo_pension_id').val()) : null,
                TipoSalarioId: 1,
                TipoJornadaId: 1,
                FechaInicio: $('#ec_fecha_inicio').val(),
                FechaFin: $('#ec_fecha_fin').val() || null,
                HorasSemanales: $('#ec_horas_semanales').val() ? Number.parseInt($('#ec_horas_semanales').val(), 10) : null,
                Salario: $('#ec_salario').val() ? Number.parseFloat($('#ec_salario').val()) : null,
                TarifaHora: $('#ec_tarifa_hora').val() ? Number.parseFloat($('#ec_tarifa_hora').val()) : null,
                ModoPago: 1,
                DescripcionFunciones: $('#ec_descripcion_funciones').val() || null,
                Observaciones: $('#ec_observaciones').val() || null
            };
        },

        
        recalcularTarifaHoraNuevo: function () {
            const salario = Number.parseFloat($('#nc_remuneracion').val());
            const horas = Number.parseInt($('#nc_horas_semanales').val(), 10);
            const tarifa = globalThis.ContratoUtils.calcularTarifaHora(salario, horas);

            $('#nc_tarifa_hora').val(tarifa || '');
        }

    };

})();