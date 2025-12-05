(function () {
    'use strict';


    function inicializarAplicacion() {
        
        globalThis.ContratoModal.init();
        
        globalThis.ContratoEvents.init();
        
        globalThis.ContratoData.cargarResumen();
        globalThis.ContratoData.cargarActivos();

        console.log('✅ Módulo de Contratos inicializado correctamente');
    }

    
    $(function () {
        inicializarAplicacion();
    });

    
    globalThis.ContratosUI = {
        recargarActivos: function () {
            globalThis.ContratoData.cargarActivos();
        },
        recargarSin: function () {
            globalThis.ContratoData.cargarSin();
        },
        recargarResumen: function () {
            globalThis.ContratoData.cargarResumen();
        },
        aplicarFiltro: function () {
            globalThis.ContratoData.aplicarFiltro();
        }
    };

})();