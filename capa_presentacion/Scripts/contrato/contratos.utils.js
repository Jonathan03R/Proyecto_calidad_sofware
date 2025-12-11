(function () {
    'use strict';

    globalThis.ContratoUtils = {


        norm: function (s) {
            return (s ?? '').toString()
                .normalize('NFD')
                .replaceAll(/[\u0300-\u036f]/g, '')
                .toLowerCase()
                .trim();
        },


        coincide: function (it, qn) {
            return !qn ||
                this.norm(it.EmpleadoNombre).includes(qn) ||
                this.norm(it.Documento).includes(qn);
        },


        fmtFecha: function (v) {
            if (!v) return '';
            const partes = String(v).split('-');
            if (partes.length !== 3) return v;

            const [yyyy, mm, dd] = partes;
            return `${dd}/${mm}/${yyyy}`;
        },


        esc: function (t) {
            if (t == null) return '';
            const m = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return String(t).replaceAll(/[&<>"']/g, s => m[s]);
        },


        emptyRow: function (colspan, texto) {
            return `<tr><td colspan="${colspan}">
                <div class="empty-state">
                    <div class="empty-state-icon">📋</div>
                    <p>${this.esc(texto)}</p>
                </div>
            </td></tr>`;
        },


        loadingRow: function (colspan) {
            return `<tr><td colspan="${colspan}">
                <div class="loading">
                    <div class="spinner"></div>
                    <p>Cargando...</p>
                </div>
            </td></tr>`;
        },

      
        tabActiva: function () {
            return $('.tab-btn.is-active').data('tab') || 'activos';
        },


        calcularTarifaHora: function (salario, horasSemanales) {
            if (!salario || !horasSemanales || horasSemanales <= 0) {
                return null;
            }

            const jornadaDiaria = horasSemanales / 6;
            if (jornadaDiaria <= 0) {
                return null;
            }

            const tarifa = salario / (30 * jornadaDiaria);
            return tarifa.toFixed(2);
        }

    };

})();