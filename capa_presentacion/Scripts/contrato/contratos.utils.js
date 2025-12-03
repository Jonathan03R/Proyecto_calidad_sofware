(function () {
    'use strict';

    // ============================================
    // MÓDULO DE UTILIDADES Y HELPERS
    // ============================================

    window.ContratoUtils = {

        /**
         * Normaliza texto para búsqueda (quita acentos y convierte a minúsculas)
         */
        norm: function (s) {
            return (s ?? '').toString()
                .normalize('NFD')
                .replace(/[\u0300-\u036f]/g, '')
                .toLowerCase()
                .trim();
        },

        /**
         * Verifica si un item coincide con la búsqueda
         */
        coincide: function (it, qn) {
            return !qn ||
                this.norm(it.EmpleadoNombre).includes(qn) ||
                this.norm(it.Documento).includes(qn);
        },

        /**
         * Formatea una fecha al formato peruano (dd/mm/yyyy)
         */
        fmtFecha: function (v) {
            if (!v) return '';
            const d = new Date(v);
            return isNaN(d)
                ? String(v)
                : d.toLocaleDateString('es-PE', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric'
                });
        },

        /**
         * Escapa caracteres HTML para prevenir XSS
         */
        esc: function (t) {
            if (t == null) return '';
            const m = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return String(t).replace(/[&<>"']/g, s => m[s]);
        },

        /**
         * Genera una fila vacía para tablas
         */
        emptyRow: function (colspan, texto) {
            return `<tr><td colspan="${colspan}">
                <div class="empty-state">
                    <div class="empty-state-icon">📋</div>
                    <p>${this.esc(texto)}</p>
                </div>
            </td></tr>`;
        },

        /**
         * Genera una fila de carga para tablas
         */
        loadingRow: function (colspan) {
            return `<tr><td colspan="${colspan}">
                <div class="loading">
                    <div class="spinner"></div>
                    <p>Cargando...</p>
                </div>
            </td></tr>`;
        },

        /**
         * Obtiene la pestaña activa actualmente
         */
        tabActiva: function () {
            return $('.tab-btn.is-active').data('tab') || 'activos';
        },

        /**
         * Recalcula la tarifa por hora basada en salario y horas semanales
         */
        calcularTarifaHora: function (salario, horasSemanales) {
            if (!salario || !horasSemanales || horasSemanales <= 0) {
                return null;
            }

            const jornadaDiaria = horasSemanales / 6.0;
            if (jornadaDiaria <= 0) {
                return null;
            }

            const tarifa = salario / (30.0 * jornadaDiaria);
            return tarifa.toFixed(2);
        }

    };

})();