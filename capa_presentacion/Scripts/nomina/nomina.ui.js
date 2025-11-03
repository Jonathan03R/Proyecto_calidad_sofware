// Interacciones de UI: carga de combos, toasts, progreso, render de tablas y modales.
(function (w, d) {
    const qs = (s) => d.querySelector(s);
    const qsa = (s) => Array.from(d.querySelectorAll(s));

    // ---------- Toasts ----------
    function toast(msg, type) {
        // type: success | danger | info | warning
        const host = qs("#toastHost") || d.body;
        const box = d.createElement("div");
        box.className = `toast-item toast-${type || "info"}`;
        box.textContent = msg;
        host.appendChild(box);
        setTimeout(() => box.classList.add("show"), 10);
        setTimeout(() => { box.classList.remove("show"); setTimeout(() => box.remove(), 300); }, 3000);
    }

    // ---------- Progreso ----------
    let _progressTimer = null;
    function startProgress() {
        const bar = qs("#progreso");
        const log = qs("#log");
        if (!bar) return;
        bar.style.width = "1%";
        bar.setAttribute("aria-valuenow", "1");
        let v = 1;
        _progressTimer = setInterval(() => {
            v = Math.min(v + Math.floor(Math.random() * 12) + 4, 95);
            bar.style.width = v + "%";
            bar.setAttribute("aria-valuenow", String(v));
            if (log) log.value += `\n[INFO] Avance ${v}%`;
            if (v >= 95) clearInterval(_progressTimer);
        }, 300);
    }
    function finishProgress() {
        const bar = qs("#progreso");
        if (!bar) return;
        bar.style.width = "100%";
        bar.setAttribute("aria-valuenow", "100");
        if (_progressTimer) clearInterval(_progressTimer);
    }

    // ---------- Render helpers ----------
    function renderPeriodos(periodos) {
        const ddl = qs("#ddlPeriodo");
        if (!ddl) return;
        ddl.innerHTML = `<option value="">-- Seleccione --</option>` +
            periodos.map(p => `<option value="${p.id}">${p.nombre} (${p.tipoPago})${p.procesado ? " ✅" : ""}</option>`).join("");
    }

    function renderVigentes(res) {
        const tbody = qs("#tblVigentesBody");
        if (!tbody) return;
        const filas = res.EmpleadosProcesados.map(e => `
      <tr>
        <td>${e.Codigo}</td>
        <td>${e.Nombre}</td>
        <td class="t-right">${e.Bruto.toFixed(2)}</td>
        <td class="t-right">${e.TotalDescuentos.toFixed(2)}</td>
        <td class="t-right">${e.Neto.toFixed(2)}</td>
      </tr>`).join("");
        tbody.innerHTML = filas || `<tr><td colspan="5" class="t-center">Sin datos</td></tr>`;
    }

    function renderErrores(res) {
        const tbody = qs("#tblErroresBody");
        if (!tbody) return;
        const filas = res.EmpleadosConError.map(e => `
      <tr class="row-error">
        <td>${e.Codigo}</td>
        <td>${e.Nombre}</td>
        <td>${e.MensajeError || "-"}</td>
      </tr>`).join("");
        tbody.innerHTML = filas || `<tr><td colspan="3" class="t-center">Sin errores</td></tr>`;
    }

    function renderResumen(res) {
        const tbody = qs("#tblResumenBody");
        if (!tbody) return;
        const filas = `
      <tr><th>Total empleados</th><td class="t-right">${res.TotalEmpleados}</td></tr>
      <tr><th>Total bruto</th><td class="t-right">${(res.TotalBruto || 0).toFixed(2)}</td></tr>
      <tr><th>Total descuentos</th><td class="t-right">${(res.TotalDescuentos || 0).toFixed(2)}</td></tr>
      <tr><th>Total neto</th><td class="t-right">${(res.TotalNeto || 0).toFixed(2)}</td></tr>
      <tr><th>Estado</th><td><span class="badge ${res.EstadoFinal === "Exitoso" ? "badge-ok" : "badge-warn"}">${res.EstadoFinal}</span></td></tr>
    `;
        tbody.innerHTML = filas;
    }

    function renderParametros(p) {
        const box = qs("#modalParametros");
        if (!box) return;
        // Asume que dentro del modal tienes spans con estos IDs
        const map = {
            RMV: p.RMV, UIT: p.UIT,
            AsigFam: (p.AsignacionFamiliarPct * 100).toFixed(2) + "%",
            EsSalud: (p.EsSaludPct * 100).toFixed(2) + "%",
            ONP: (p.ONPPct * 100).toFixed(2) + "%",
            AFPF: (p.AFPFondoPct * 100).toFixed(2) + "%",
            AFPC: (p.AFPComisionPct * 100).toFixed(2) + "%",
            AFPS: (p.AFPSeguroPct * 100).toFixed(2) + "%"
        };
        Object.keys(map).forEach(k => {
            const el = box.querySelector(`[data-param="${k}"]`);
            if (el) el.textContent = map[k];
        });
        // Tabla IR
        const tbody = box.querySelector("tbody");
        if (tbody) {
            tbody.innerHTML = (p.TablaIR || []).map(t =>
                `<tr><td>${t.DesdeUIT}</td><td>${t.HastaUIT === 0 ? "Sin tope" : t.HastaUIT}</td><td>${t.TasaPct}%</td></tr>`
            ).join("");
        }
    }

    // ---------- Eventos ----------
    async function init() {
        try {
            // Cargar periodos en el combo
            const periodos = await w.NominaAPI.getPeriods();
            renderPeriodos(periodos);

            // Botón Parámetros
            const btnPar = qs("#btnParametros");
            if (btnPar) {
                btnPar.addEventListener("click", async () => {
                    const data = await w.NominaAPI.getParameters();
                    renderParametros(data);
                    // Si usas modal CSS propio, alterna una clase visible
                    qs("#modalParametros")?.classList.add("show");
                });
            }

            // Cerrar modal (si usas un botón con data-close)
            qsa("[data-close='modalParametros']").forEach(b => {
                b.addEventListener("click", () => qs("#modalParametros")?.classList.remove("show"));
            });

            // Botón Procesar
            const btnProc = qs("#btnProcesar");
            if (btnProc) {
                btnProc.addEventListener("click", async (ev) => {
                    ev.preventDefault();
                    if (!w.NominaValidate.validarPeriodo()) { toast("Seleccione un período.", "warning"); return; }

                    const periodoId = qs("#ddlPeriodo").value;
                    // Bloqueo simple
                    btnProc.disabled = true;
                    startProgress();
                    qs("#log") && (qs("#log").value = "[INFO] Iniciando procesamiento...");

                    try {
                        const res = await w.NominaAPI.simulateProcess(periodoId);
                        finishProgress();
                        renderVigentes(res);
                        renderErrores(res);
                        renderResumen(res);
                        toast(res.MensajeEstado, res.EstadoFinal === "Exitoso" ? "success" : "warning");
                    } catch (e) {
                        finishProgress();
                        toast("Ocurrió un error simulando el proceso.", "danger");
                    } finally {
                        btnProc.disabled = false;
                    }
                });
            }
        } catch (e) {
            console.error(e);
            toast("No se pudo inicializar la UI.", "danger");
        }
    }

    // Iniciar cuando el DOM está listo
    if (d.readyState === "loading") d.addEventListener("DOMContentLoaded", init);
    else init();

})(window, document);
