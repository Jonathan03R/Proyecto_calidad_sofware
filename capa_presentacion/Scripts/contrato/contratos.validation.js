(function () {

    'use strict';

    globalThis.ContratoValidation = {

        validarNuevoContrato: function (contrato) {

            
            if (!contrato.TipoSalarioId) {
                contrato.TipoSalarioId = 1;
            }

            if (!contrato.TrabajadorId) {
                return { valido: false, mensaje: 'Falta el trabajador.' };
            }

            if (!contrato.CargoId) {
                return { valido: false, mensaje: 'Seleccione un cargo.' };
            }

            if (!contrato.AreaId) {
                return { valido: false, mensaje: 'Seleccione un área.' };
            }

            if (!contrato.TipoPensionId) {
                return { valido: false, mensaje: 'Seleccione el sistema de pensiones.' };
            }

            if (!contrato.FechaInicio) {
                return { valido: false, mensaje: 'Ingrese la fecha de inicio.' };
            }

            if (!contrato.TipoJornadaId) {
                contrato.TipoJornadaId = 1;
            }

            if (!contrato.Salario || Number.isNaN(contrato.Salario) || contrato.Salario <= 0) {
                return { valido: false, mensaje: 'Ingrese un salario mayor a 0.' };
            }

            if (!contrato.HorasSemanales || Number.isNaN(contrato.HorasSemanales) || contrato.HorasSemanales <= 0) {
                return { valido: false, mensaje: 'Ingrese las horas semanales (mayores a 0).' };
            }

            const validacionFechas = this.validarFechas(contrato.FechaInicio, contrato.FechaFin);
            if (!validacionFechas.valido) {
                return validacionFechas;
            }

            const validacionDuracion = this.validarDuracionMinima(contrato.FechaInicio, contrato.FechaFin);
            if (!validacionDuracion.valido) {
                return validacionDuracion;
            }

            return { valido: true, mensaje: '' };
        },

        validarEdicionContrato: function (data, motivo) {

            if (!data.TipoSalarioId) {
                data.TipoSalarioId = 1;
            }

            if (!motivo || motivo.trim() === '') {
                return { valido: false, mensaje: 'Ingrese el motivo de la actualización.' };
            }

            if (data.Salario == null || Number.isNaN(data.Salario) || data.Salario <= 0) {
                return { valido: false, mensaje: 'Ingrese un salario mayor a 0.' };
            }

            const validacionFechas = this.validarFechas(data.FechaInicio, data.FechaFin);
            if (!validacionFechas.valido) {
                return validacionFechas;
            }

            return { valido: true, mensaje: '' };
        },

        validarFechas: function (fechaInicio, fechaFin) {
            const fIni = fechaInicio ? new Date(fechaInicio) : null;
            const fFin = fechaFin ? new Date(fechaFin) : null;

            if (fIni && fFin && fFin < fIni) {
                return {
                    valido: false,
                    mensaje: 'La fecha de fin no puede ser anterior a la fecha de inicio.'
                };
            }

            return { valido: true, mensaje: '' };
        },

        validarDuracionMinima: function (fechaInicio, fechaFin) {
            const fIni = fechaInicio ? new Date(fechaInicio) : null;
            const fFin = fechaFin ? new Date(fechaFin) : null;

            if (fIni && fFin) {
                const diffMeses = this.calcularDiferenciaMeses(fIni, fFin);

                if (diffMeses < 3) {
                    return {
                        valido: false,
                        mensaje: 'El tiempo mínimo de contrato debe ser de al menos 3 meses.'
                    };
                }
            }

            return { valido: true, mensaje: '' };
        },

        calcularDiferenciaMeses: function (fechaInicio, fechaFin) {
            return (fechaFin.getFullYear() - fechaInicio.getFullYear()) * 12 +
                (fechaFin.getMonth() - fechaInicio.getMonth());
        },

        validarNumero: function (valor, nombreCampo, requerido = true) {
            if (requerido && (valor == null || valor === '')) {
                return {
                    valido: false,
                    mensaje: `${nombreCampo} es requerido.`
                };
            }

            if (valor != null && valor !== '' && (Number.isNaN(valor) || Number(valor) <= 0)) {
                return {
                    valido: false,
                    mensaje: `${nombreCampo} debe ser un número mayor a 0.`
                };
            }

            return { valido: true, mensaje: '' };
        },

        validarSeleccion: function (valor, nombreCampo) {
            if (!valor || valor === '') {
                return {
                    valido: false,
                    mensaje: `Seleccione ${nombreCampo}.`
                };
            }

            return { valido: true, mensaje: '' };
        }

    };

})();