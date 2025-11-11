// nomina.ui.js - VERSIÓN COMPLETAMENTE CORREGIDA
console.log("✅ nomina.ui.js se está cargando...");

document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ DOM cargado, inicializando...");
    init();
});

async function init() {
    try {
        console.log("🔄 Inicializando nómina UI...");

        // 1. Cargar períodos
        const response = await fetch("/Nomina/ListarPeriodos");
        const periodos = await response.json();
        console.log("Períodos cargados:", periodos);

        // 2. Llenar el combo
        const combo = document.getElementById("ddlPeriodo");
        combo.innerHTML = "";

        periodos.forEach(p => {
            const option = document.createElement("option");
            option.value = p.Id;
            option.textContent = p.Nombre;
            combo.appendChild(option);
        });

        // 3. Configurar el botón de procesar - ID CORREGIDO
        const btnProcesar = document.getElementById("btnProcesar");
        if (btnProcesar) {
            console.log("✅ Botón encontrado:", btnProcesar);
            btnProcesar.addEventListener("click", function (ev) {
                ev.preventDefault();
                console.log("🔄 Botón procesar clickeado");

                // Validar período
                const periodoSelect = document.getElementById("ddlPeriodo");
                if (!periodoSelect.value) {
                    alert("❌ Por favor selecciona un período");
                    return;
                }

                const periodoId = periodoSelect.value;
                console.log("📤 Enviando período:", periodoId);

                // URL CORREGIDA: /Nomina/ProcesarNomina (con C)
                $.post("/Nomina/ProcesarNomina", {
                    periodoId: periodoId
                })
                    .done(function (resp) {
                        console.log("✅ Respuesta del servidor:", resp);
                        if (resp.ok) {
                            alert("✅ " + resp.msg);
                        } else {
                            alert("❌ " + resp.msg);
                        }
                    })
                    .fail(function (xhr, status, error) {
                        console.error("❌ Error en POST:", {
                            status: status,
                            error: error,
                            responseText: xhr.responseText
                        });
                        alert("❌ Error de conexión: " + error);
                    });
            });
        } else {
            console.error("❌ Botón btnProcesar no encontrado");
        }

        console.log("✅ UI inicializada correctamente");

    } catch (error) {
        console.error("❌ Error en init:", error);
        alert("Error al inicializar la página: " + error.message);
    }
}