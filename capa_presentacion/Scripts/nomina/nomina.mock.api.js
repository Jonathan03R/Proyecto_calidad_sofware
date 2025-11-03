// Simulación de "API" para poblar la UI sin BD ni capa aplicación.
(function (w) {
    const delay = (ms) => new Promise(r => setTimeout(r, ms));

    const _periodos = [
        { id: "2025-09", nombre: "Septiembre 2025", tipoPago: "Mensual", procesado: true },
        { id: "2025-10", nombre: "Octubre 2025", tipoPago: "Mensual", procesado: true },
        { id: "2025-11", nombre: "Noviembre 2025", tipoPago: "Mensual", procesado: false },
        { id: "2025-11-Q2", nombre: "Quincena 2 - Nov 2025", tipoPago: "Quincenal", procesado: false },
    ];

    const _parametros = {
        RMV: 1025, UIT: 5150, AsignacionFamiliarPct: 0.10, EsSaludPct: 0.09,
        ONPPct: 0.13, AFPFondoPct: 0.10, AFPComisionPct: 0.015, AFPSeguroPct: 0.017,
        TablaIR: [
            { DesdeUIT: 0, HastaUIT: 5, TasaPct: 0, DeduccionFija: 0 },
            { DesdeUIT: 5, HastaUIT: 20, TasaPct: 8, DeduccionFija: 0 },
            { DesdeUIT: 20, HastaUIT: 35, TasaPct: 14, DeduccionFija: 0 },
            { DesdeUIT: 35, HastaUIT: 45, TasaPct: 17, DeduccionFija: 0 },
            { DesdeUIT: 45, HastaUIT: 0, TasaPct: 20, DeduccionFija: 0 }, // 0 = sin tope
        ]
    };

    function _empleadoOk(codigo, nombre, basico, esOnp = true) {
        const onp = esOnp ? basico * 0.13 : 0;
        const afpF = esOnp ? 0 : basico * 0.10;
        const afpC = esOnp ? 0 : basico * 0.015;
        const afpS = esOnp ? 0 : basico * 0.017;
        const horasExtras = 120, bonos = 150, otros = 0;
        const bruto = basico + horasExtras + bonos + otros;
        const renta = 80, adelantos = 0, tard = 0, falt = 0, otrosDesc = 0;
        const totalDesc = onp + afpF + afpC + afpS + renta + adelantos + tard + falt + otrosDesc;
        const essalud = basico * 0.09;

        return {
            Codigo: codigo, Documento: "DNI" + String(codigo).padStart(8, "0"), Nombre: nombre,
            SueldoBasico: basico, HorasExtras: horasExtras, Bonos: bonos, OtrosIngresos: otros,
            Bruto: bruto, EsONP: esOnp, DescuentoONP: onp, DescuentoAFP_Fondo: afpF,
            DescuentoAFP_Comision: afpC, DescuentoAFP_Seguro: afpS, ImpuestoRentaMes: renta,
            Adelantos: adelantos, Tardanzas: tard, Faltas: falt, OtrosDescuentos: otrosDesc,
            TotalDescuentos: totalDesc, EsSalud: essalud, Neto: bruto - totalDesc,
            TieneErrores: false, MensajeError: null
        };
    }

    function _empleadoError(codigo, nombre, msg) {
        const e = _empleadoOk(codigo, nombre, 1300, true);
        e.TieneErrores = true; e.MensajeError = msg;
        return e;
    }

    w.NominaAPI = {
        getPeriods: async () => {
            await delay(250);
            return _periodos.slice();
        },

        getParameters: async () => {
            await delay(200);
            return JSON.parse(JSON.stringify(_parametros));
        },

        simulateProcess: async (periodoId) => {
            // Simula tiempo de proceso y payload final
            await delay(500);
            const p = _periodos.find(x => x.id === periodoId) || _periodos.find(x => !x.procesado) || _periodos[0];

            const ok = [
                _empleadoOk(101, "Ana Rojas", 1600, true),
                _empleadoOk(102, "Carlos Díaz", 1800, false),
                _empleadoOk(103, "María Pérez", 1500, false),
            ];
            const err = [
                _empleadoError(201, "Luis Castro", "Falta contrato activo"),
                _empleadoError(202, "Julia Vega", "Parámetros oficiales incompletos"),
            ];

            const totalBruto = ok.reduce((s, e) => s + e.Bruto, 0);
            const totalDesc = ok.reduce((s, e) => s + e.TotalDescuentos, 0);
            const totalNeto = ok.reduce((s, e) => s + e.Neto, 0);

            return {
                PeriodoId: p.id, PeriodoNombre: p.nombre, TipoPago: p.tipoPago,
                EmpleadosProcesados: ok, EmpleadosConError: err,
                TotalEmpleados: ok.length + err.length,
                TotalBruto: totalBruto, TotalDescuentos: totalDesc, TotalNeto: totalNeto,
                EstadoFinal: err.length ? "Con errores" : "Exitoso",
                MensajeEstado: err.length ? "Se procesó parcialmente. Revise la pestaña de Errores."
                    : "Procesamiento completado correctamente."
            };
        },

        getHistory: async () => {
            await delay(250);
            return [
                { PeriodoId: "2025-09", PeriodoNombre: "Septiembre 2025", TipoPago: "Mensual", YaProcesado: true, PuedeProcesar: false },
                { PeriodoId: "2025-10", PeriodoNombre: "Octubre 2025", TipoPago: "Mensual", YaProcesado: true, PuedeProcesar: false },
                { PeriodoId: "2025-11", PeriodoNombre: "Noviembre 2025", TipoPago: "Mensual", YaProcesado: false, PuedeProcesar: true },
            ];
        }
    };
})(window);
