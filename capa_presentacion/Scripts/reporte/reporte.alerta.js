
const Alertas = (function () {
    'use strict';

    // ===== CONFIGURACIÓN =====
    const CONFIG = {
        duracionPorDefecto: 5000,
        animacionDuracion: 300
    };

    // ===== ESTADO PRIVADO =====
    let elementosCache = {
        success: null,
        error: null
    };

    /**
     * Inicializa las referencias DOM
     * Se ejecuta automáticamente al cargar la página
     */
    function init() {
        elementosCache.success = $('#alertSuccess');
        elementosCache.error = $('#alertError');
    }

    /**
     * Muestra un mensaje de alerta
     * @param {string} tipo - Tipo de alerta: 'success' o 'error'
     * @param {string} mensaje - Mensaje a mostrar
     * @param {number} duracion - Duración en ms (opcional)
     */
    function mostrar(tipo, mensaje, duracion = CONFIG.duracionPorDefecto) {
        // Inicializar si no se ha hecho
        if (!elementosCache.success) {
            init();
        }

        // Validar parámetros
        if (!mensaje || typeof mensaje !== 'string') {
            console.error('Alertas: El mensaje debe ser una cadena de texto');
            return;
        }

        // Ocultar todas las alertas primero
        ocultarTodas();

        // Obtener el elemento correcto
        const elemento = tipo === 'success' ? elementosCache.success : elementosCache.error;
        const icono = tipo === 'success' ? '✓' : '✕';

        // Mostrar la alerta
        elemento.text(`${icono} ${mensaje}`).addClass('show');

        // Auto-ocultar después del tiempo especificado
        if (duracion > 0) {
            setTimeout(() => {
                ocultar(tipo);
            }, duracion);
        }
    }

    /**
     * Muestra un mensaje de éxito
     * @param {string} mensaje - Mensaje a mostrar
     * @param {number} duracion - Duración en ms (opcional)
     */
    function exito(mensaje, duracion) {
        mostrar('success', mensaje, duracion);
    }

    /**
     * Muestra un mensaje de error
     * @param {string} mensaje - Mensaje a mostrar
     * @param {number} duracion - Duración en ms (opcional)
     */
    function error(mensaje, duracion) {
        mostrar('error', mensaje, duracion);
    }

    /**
     * Oculta una alerta específica
     * @param {string} tipo - Tipo de alerta: 'success' o 'error'
     */
    function ocultar(tipo) {
        const elemento = tipo === 'success' ? elementosCache.success : elementosCache.error;
        if (elemento) {
            elemento.removeClass('show');
        }
    }

    /**
     * Oculta todas las alertas visibles
     */
    function ocultarTodas() {
        if (elementosCache.success) elementosCache.success.removeClass('show');
        if (elementosCache.error) elementosCache.error.removeClass('show');
    }

    /**
     * Muestra un mensaje de carga (no se oculta automáticamente)
     * @param {string} mensaje - Mensaje de carga
     */
    function cargando(mensaje = 'Cargando...') {
        mostrar('success', mensaje, 0); // Duración 0 = no se oculta automáticamente
    }

    /**
     * Muestra un diálogo de confirmación
     * @param {string} mensaje - Mensaje de confirmación
     * @param {Function} onConfirm - Callback si el usuario confirma
     * @param {Function} onCancel - Callback si el usuario cancela (opcional)
     */
    function confirmar(mensaje, onConfirm, onCancel) {
        if (confirm(mensaje)) {
            if (typeof onConfirm === 'function') {
                onConfirm();
            }
        } else {
            if (typeof onCancel === 'function') {
                onCancel();
            }
        }
    }

    /**
     * Muestra una alerta de validación
     * @param {string} mensaje - Mensaje de validación
     */
    function validacion(mensaje) {
        error(`Validación: ${mensaje}`, 4000);
    }

    /**
     * Muestra una alerta de información
     * @param {string} mensaje - Mensaje informativo
     */
    function info(mensaje) {
        exito(`ℹ ${mensaje}`, 4000);
    }

    // ===== API PÚBLICA =====
    return {
        init: init,
        mostrar: mostrar,
        exito: exito,
        error: error,
        ocultar: ocultar,
        ocultarTodas: ocultarTodas,
        cargando: cargando,
        confirmar: confirmar,
        validacion: validacion,
        info: info
    };
})();

// Auto-inicializar cuando el DOM esté listo
$(document).ready(function () {
    Alertas.init();
});