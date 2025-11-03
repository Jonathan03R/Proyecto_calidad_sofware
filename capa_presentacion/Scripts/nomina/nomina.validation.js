// Validaciones simples de formulario y helpers visuales.
(function (w, d) {
    const qs = (s) => d.querySelector(s);

    function setError(el, msg) {
        if (!el) return;
        el.classList.add("is-invalid");
        let fb = el.parentElement.querySelector(".invalid-feedback");
        if (!fb) {
            fb = d.createElement("div");
            fb.className = "invalid-feedback";
            el.parentElement.appendChild(fb);
        }
        fb.textContent = msg || "Campo inválido";
    }

    function clearError(el) {
        if (!el) return;
        el.classList.remove("is-invalid");
        const fb = el.parentElement.querySelector(".invalid-feedback");
        if (fb) fb.textContent = "";
    }

    function validarPeriodo() {
        const ddl = qs("#ddlPeriodo");
        clearError(ddl);
        if (!ddl || !ddl.value) { setError(ddl, "Seleccione un período."); return false; }
        return true;
    }

    // Exponer para uso desde nomina.ui.js
    w.NominaValidate = {
        validarPeriodo,
        setError,
        clearError
    };
})(window, document);
