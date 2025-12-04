(function () {
    'use strict';

    // ============================================
    // MÓDULO PRINCIPAL - INICIALIZACIÓN
    // ============================================

    /**
     * Inicializa la aplicación de contratos
     */
    function inicializarAplicacion() {
        // Inicializar modales
        window.ContratoModal.init();

        // Inicializar eventos
        window.ContratoEvents.init();

        // Cargar datos iniciales
        window.ContratoData.cargarResumen();
        window.ContratoData.cargarActivos();

        console.log('✅ Módulo de Contratos inicializado correctamente');
    }

    // Ejecutar cuando el DOM esté listo
    $(function () {
        inicializarAplicacion();
    });

    // Exponer API pública para recargas manuales
    window.ContratosUI = {
        recargarActivos: function () {
            window.ContratoData.cargarActivos();
        },
        recargarSin: function () {
            window.ContratoData.cargarSin();
        },
        recargarResumen: function () {
            window.ContratoData.cargarResumen();
        },
        aplicarFiltro: function () {
            window.ContratoData.aplicarFiltro();
        }
    };

})();