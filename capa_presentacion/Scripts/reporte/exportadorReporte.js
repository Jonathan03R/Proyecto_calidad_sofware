const ExportadorReportes = (function () {
    'use strict';

    // ===== CONFIGURACIÓN =====
    const CONFIG = {
        timeout: 30000,
        extensiones: {
            pdf: 'pdf'
        },
        mimeTypes: {
            pdf: 'application/pdf'
        }
    };

    // ===== DESCARGA DE ARCHIVOS CON AJAX =====
    function descargarArchivo({ url, nombreArchivo, tipoMime, onSuccess, onError, onFinally }) {
        $.ajax({
            url: url,
            type: 'GET',
            xhrFields: {
                responseType: 'blob'
            },
            timeout: CONFIG.timeout,
            success: function (blob, status, xhr) {
                try {
                    // Obtener el content-type del header o usar el proporcionado
                    const contentType = xhr.getResponseHeader('content-type') || tipoMime;

                    // Crear blob con el tipo correcto
                    const file = new Blob([blob], { type: contentType });
                    const urlBlob = window.URL.createObjectURL(file);
                    const link = document.createElement('a');
                    link.href = urlBlob;
                    link.download = nombreArchivo;

                    // Añadir al DOM, hacer clic y remover
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);

                    // Liberar memoria después de un breve delay
                    setTimeout(() => window.URL.revokeObjectURL(urlBlob), 100);

                    if (onSuccess) onSuccess(nombreArchivo);
                } catch (error) {
                    console.error('Error al procesar la descarga:', error);
                    if (onError) onError('Error al procesar el archivo descargado');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error en la descarga:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    error: error
                });

                let mensajeError = 'Error al descargar el archivo';

                // Intentar obtener mensaje del servidor
                if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                    mensajeError = xhr.responseJSON.mensaje;
                } else if (xhr.responseText) {
                    try {
                        const errorData = JSON.parse(xhr.responseText);
                        mensajeError = errorData.mensaje || errorData.error || mensajeError;
                    } catch (e) {
                        // No se pudo parsear
                    }
                }

                // Mensajes específicos según código de error
                if (xhr.status === 404) {
                    mensajeError = 'El endpoint de exportación no existe';
                } else if (xhr.status === 500) {
                    mensajeError = 'Error en el servidor al generar el reporte';
                } else if (xhr.status === 0) {
                    mensajeError = 'No se pudo conectar con el servidor';
                } else if (status === 'timeout') {
                    mensajeError = 'La solicitud excedió el tiempo límite';
                }

                if (onError) onError(mensajeError);
            },
            complete: function () {
                if (onFinally) onFinally();
            }
        });
    }

    // ===== EXPORTACIÓN PDF =====
    function exportarPDF(params) {
        const {
            url,
            periodoId,
            cargoId = null,
            nombreArchivo = 'reporte_nomina',
            onStart = null,
            onSuccess = null,
            onError = null,
            onFinally = null
        } = params;

        if (!validarParametros({ url, periodoId })) {
            if (onError) onError('Parámetros inválidos');
            return;
        }

        if (onStart) onStart();

        const urlCompleta = construirUrl(url, { periodoId, cargoId });
        const nombreCompleto = `${nombreArchivo}_${obtenerFechaHora()}.${CONFIG.extensiones.pdf}`;

        descargarArchivo({
            url: urlCompleta,
            nombreArchivo: nombreCompleto,
            tipoMime: CONFIG.mimeTypes.pdf,
            onSuccess,
            onError,
            onFinally
        });
    }

    // ===== UTILIDADES =====
    function construirUrl(baseUrl, params) {
        const queryParams = new URLSearchParams();

        Object.keys(params).forEach(key => {
            if (params[key] !== null && params[key] !== undefined && params[key] !== '') {
                queryParams.append(key, params[key]);
            }
        });

        const queryString = queryParams.toString();
        return queryString ? `${baseUrl}?${queryString}` : baseUrl;
    }

    function obtenerFechaHora() {
        const ahora = new Date();
        const year = ahora.getFullYear();
        const month = String(ahora.getMonth() + 1).padStart(2, '0');
        const day = String(ahora.getDate()).padStart(2, '0');
        const hours = String(ahora.getHours()).padStart(2, '0');
        const minutes = String(ahora.getMinutes()).padStart(2, '0');
        const seconds = String(ahora.getSeconds()).padStart(2, '0');

        return `${year}${month}${day}_${hours}${minutes}${seconds}`;
    }

    function validarParametros(params) {
        const { url, periodoId } = params;

        if (!url) {
            console.error('URL es requerida para exportar');
            return false;
        }

        if (!periodoId) {
            console.error('periodoId es requerido para exportar');
            return false;
        }

        return true;
    }

    // ===== API PÚBLICA =====
    return {
        exportarPDF,
        construirUrl,
        obtenerFechaHora
    };

})();

// Hacer disponible globalmente
if (typeof window !== 'undefined') {
    window.ExportadorReportes = ExportadorReportes;
    console.log('✅ ExportadorReportes cargado correctamente');
}