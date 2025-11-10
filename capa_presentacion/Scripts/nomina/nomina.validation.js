// nomina.validation.js
console.log("✅ nomina.validation.js cargándose...");

(function (global) {
    'use strict';

    const NominaValidate = {
        validarPeriodo: function () {
            const ddl = document.querySelector("#ddlPeriodo");
            if (!ddl || !ddl.value) {
                console.warn("❌ Validación falló: No hay período seleccionado");
                return false;
            }
            console.log("✅ Validación pasó: Período seleccionado:", ddl.value);
            return true;
        },

        setError: function (element, message) {
            if (element) {
                element.classList.add("is-invalid");
                console.warn("❌ Error establecido:", message);
            }
        },

        clearError: function (element) {
            if (element) {
                element.classList.remove("is-invalid");
            }
        }
    };

    // Exponer globalmente
    global.NominaValidate = NominaValidate;
    console.log("✅ NominaValidate inicializado:", NominaValidate);

})(window);

console.log("✅ nomina.validation.js cargado completamente");