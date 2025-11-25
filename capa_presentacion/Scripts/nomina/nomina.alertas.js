// nomina.alertas.js
(function (global) {
    'use strict';

    const Alertas = (function () {
        let contenedorAlertas = null;

        function init() {
            if (!document.getElementById('alertas-container')) {
                contenedorAlertas = document.createElement('div');
                contenedorAlertas.id = 'alertas-container';
                contenedorAlertas.style.cssText = `
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 9999;
                    max-width: 400px;
                `;
                document.body.appendChild(contenedorAlertas);
            } else {
                contenedorAlertas = document.getElementById('alertas-container');
            }
        }

        function mostrarAlerta(mensaje, tipo, duracion = 4000) {
            if (!contenedorAlertas) init();

            const colores = {
                exito: { bg: '#d4edda', border: '#c3e6cb', texto: '#155724', icono: '✓' },
                error: { bg: '#f8d7da', border: '#f5c6cb', texto: '#721c24', icono: '✕' },
                info: { bg: '#d1ecf1', border: '#bee5eb', texto: '#0c5460', icono: 'ℹ' },
                validacion: { bg: '#fff3cd', border: '#ffeaa7', texto: '#856404', icono: '⚠' },
                cargando: { bg: '#e3f2fd', border: '#90caf9', texto: '#1565c0', icono: '⟳' }
            };

            const config = colores[tipo] || colores.info;

            const alerta = document.createElement('div');
            alerta.className = 'alerta-nomina';
            alerta.style.cssText = `
                background: ${config.bg};
                border: 1px solid ${config.border};
                color: ${config.texto};
                padding: 15px 20px;
                border-radius: 8px;
                margin-bottom: 10px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                display: flex;
                align-items: center;
                animation: slideIn 0.3s ease-out;
                font-size: 14px;
                font-weight: 500;
            `;

            const icono = document.createElement('span');
            const anim = tipo === 'cargando' ? 'animation: spin 1s linear infinite;' : '';
            icono.style.cssText = `
                font-size: 20px;
                margin-right: 12px;
                ${anim}
            `;
            icono.textContent = config.icono;

            const texto = document.createElement('span');
            texto.textContent = mensaje;
            texto.style.flex = '1';

            const btnCerrar = document.createElement('button');
            btnCerrar.innerHTML = '×';
            btnCerrar.style.cssText = `
                background: none;
                border: none;
                font-size: 24px;
                color: ${config.texto};
                cursor: pointer;
                padding: 0;
                margin-left: 15px;
                line-height: 1;
                opacity: 0.7;
                transition: opacity 0.2s;
            `;
            btnCerrar.onmouseover = () => btnCerrar.style.opacity = '1';
            btnCerrar.onmouseout = () => btnCerrar.style.opacity = '0.7';
            btnCerrar.onclick = () => cerrarAlerta(alerta);

            alerta.appendChild(icono);
            alerta.appendChild(texto);
            if (tipo !== 'cargando') {
                alerta.appendChild(btnCerrar);
            }

            contenedorAlertas.appendChild(alerta);

            if (tipo !== 'cargando' && duracion > 0) {
                setTimeout(() => cerrarAlerta(alerta), duracion);
            }

            return alerta;
        }

        function cerrarAlerta(alerta) {
            if (!alerta || !alerta.parentNode) return;

            alerta.style.animation = 'slideOut 0.3s ease-in';
            setTimeout(() => {
                if (alerta.parentNode) {
                    alerta.parentNode.removeChild(alerta);
                }
            }, 300);
        }

        function ocultarTodas() {
            if (!contenedorAlertas) return;

            const alertas = contenedorAlertas.querySelectorAll('.alerta-nomina');
            alertas.forEach(alerta => cerrarAlerta(alerta));
        }

        // Estilos globales una sola vez
        if (!document.getElementById('alertas-styles')) {
            const styles = document.createElement('style');
            styles.id = 'alertas-styles';
            styles.textContent = `
                @keyframes slideIn {
                    from {
                        transform: translateX(400px);
                        opacity: 0;
                    }
                    to {
                        transform: translateX(0);
                        opacity: 1;
                    }
                }
                @keyframes slideOut {
                    from {
                        transform: translateX(0);
                        opacity: 1;
                    }
                    to {
                        transform: translateX(400px);
                        opacity: 0;
                    }
                }
                @keyframes spin {
                    from { transform: rotate(0deg); }
                    to { transform: rotate(360deg); }
                }
                .btn-ver-detalle {
                    padding: 6px 12px;
                    border: 1px solid #1976d2;
                    background: white;
                    color: #1976d2;
                    border-radius: 4px;
                    cursor: pointer;
                    font-size: 11px;
                    transition: all 0.2s;
                }
                .btn-ver-detalle:hover {
                    background: #1976d2;
                    color: white;
                }
            `;
            document.head.appendChild(styles);
        }

        return {
            exito: (msg, dur) => mostrarAlerta(msg, 'exito', dur),
            error: (msg, dur) => mostrarAlerta(msg, 'error', dur),
            info: (msg, dur) => mostrarAlerta(msg, 'info', dur),
            validacion: (msg, dur) => mostrarAlerta(msg, 'validacion', dur),
            cargando: (msg) => mostrarAlerta(msg, 'cargando', 0),
            ocultarTodas: ocultarTodas
        };
    })();

    global.Alertas = Alertas;
    console.log('✅ nomina.alertas.js cargado');

})(window);
