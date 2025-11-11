// ============================================
// GESTIÓN DE CONTRATOS - JAVASCRIPT
// ============================================

// ✅ INICIALIZACIÓN AL CARGAR LA PÁGINA
document.addEventListener('DOMContentLoaded', function () {
    inicializarEventos();
});

// Función para inicializar todos los event listeners
function inicializarEventos() {
    // Event listeners para los botones "Ver lista"
    document.querySelectorAll('.btn-primary[data-action]').forEach(button => {
        button.addEventListener('click', function () {
            const action = this.getAttribute('data-action');
            manejarAccion(action);
        });
    });

    // Event listener para cerrar modal
    const closeButton = document.querySelector('.close[data-action="cerrar"]');
    if (closeButton) {
        closeButton.addEventListener('click', cerrarModal);
    }

    // Cerrar modal al hacer clic fuera de él
    window.addEventListener('click', function (event) {
        const modal = document.getElementById('modalLista');
        if (event.target == modal) {
            cerrarModal();
        }
    });
}

// Manejador central de acciones
function manejarAccion(accion) {
    switch (accion) {
        case 'trabajadores':
            verListaTrabajadores();
            break;
        case 'areas':
            verListaAreas();
            break;
        case 'cargos':
            verListaCargos();
            break;
        case 'pensiones':
            verListaPensiones();
            break;
        case 'estados':
            verListaEstados();
            break;
        default:
            console.error('Acción no reconocida:', accion);
    }
}

// Función para abrir el modal
function abrirModal(titulo) {
    document.getElementById('modalTitulo').innerHTML = titulo;
    document.getElementById('modalLista').style.display = 'block';
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('modalContenido').innerHTML = '';
}

// Función para cerrar el modal
function cerrarModal() {
    document.getElementById('modalLista').style.display = 'none';
}

// Cerrar modal al hacer clic fuera de él
window.onclick = function (event) {
    const modal = document.getElementById('modalLista');
    if (event.target == modal) {
        cerrarModal();
    }
}

// Función para ocultar el loading
function ocultarLoading() {
    document.getElementById('loadingSpinner').style.display = 'none';
}

// Función para mostrar error
function mostrarError(mensaje) {
    ocultarLoading();
    console.error('Error detallado:', mensaje); // Para debugging
    document.getElementById('modalContenido').innerHTML = `
        <div style="text-align: center; padding: 40px; color: #d9534f;">
            <h3>❌ Error</h3>
            <p>${mensaje}</p>
            <button onclick="cerrarModal()" style="margin-top: 20px; padding: 10px 20px; background: #667eea; color: white; border: none; border-radius: 5px; cursor: pointer;">Cerrar</button>
        </div>
    `;
}

// ============================================
// VER LISTA DE TRABAJADORES
// ============================================
function verListaTrabajadores() {
    abrirModal('👥 Listado de Trabajadores');

    console.log('Solicitando trabajadores...'); // Debug

    fetch('/Contratos/ObtenerTrabajadores')
        .then(response => {
            console.log('Response status:', response.status); // Debug
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log('Datos recibidos:', data); // Debug
            ocultarLoading();
            if (data.success) {
                if (data.data.length === 0) {
                    document.getElementById('modalContenido').innerHTML = `
                        <div style="text-align: center; padding: 40px; color: #999;">
                            <p>📭 No hay trabajadores registrados</p>
                        </div>
                    `;
                    return;
                }

                let html = '<table class="data-table"><thead><tr>';
                html += '<th>ID</th>';
                html += '<th>Nombre Completo</th>';
                html += '</tr></thead><tbody>';

                data.data.forEach(trabajador => {
                    html += '<tr>';
                    html += `<td>${trabajador.TrabajadorId}</td>`;
                    html += `<td>${trabajador.NombreCompleto}</td>`;
                    html += '</tr>';
                });

                html += '</tbody></table>';
                document.getElementById('modalContenido').innerHTML = html;
            } else {
                mostrarError(data.message);
            }
        })
        .catch(error => {
            mostrarError('Error al cargar los trabajadores: ' + error);
        });
}

// ============================================
// VER LISTA DE ÁREAS
// ============================================
function verListaAreas() {
    abrirModal('🏢 Áreas de la Empresa');

    fetch('/Contratos/ObtenerAreas')
        .then(response => response.json())
        .then(data => {
            ocultarLoading();
            if (data.success) {
                if (data.data.length === 0) {
                    document.getElementById('modalContenido').innerHTML = `
                        <div style="text-align: center; padding: 40px; color: #999;">
                            <p>📭 No hay áreas registradas</p>
                        </div>
                    `;
                    return;
                }

                let html = '<table class="data-table"><thead><tr>';
                html += '<th>ID</th>';
                html += '<th>Nombre del Área</th>';
                html += '</tr></thead><tbody>';

                data.data.forEach(area => {
                    html += '<tr>';
                    html += `<td>${area.AreaId}</td>`;
                    html += `<td>${area.NombreArea}</td>`;
                    html += '</tr>';
                });

                html += '</tbody></table>';
                document.getElementById('modalContenido').innerHTML = html;
            } else {
                mostrarError(data.message);
            }
        })
        .catch(error => {
            mostrarError('Error al cargar las áreas: ' + error);
        });
}

// ============================================
// VER LISTA DE CARGOS
// ============================================
function verListaCargos() {
    abrirModal('💼 Cargos Disponibles');

    fetch('/Contratos/ObtenerCargos')
        .then(response => response.json())
        .then(data => {
            ocultarLoading();
            if (data.success) {
                if (data.data.length === 0) {
                    document.getElementById('modalContenido').innerHTML = `
                        <div style="text-align: center; padding: 40px; color: #999;">
                            <p>📭 No hay cargos registrados</p>
                        </div>
                    `;
                    return;
                }

                let html = '<table class="data-table"><thead><tr>';
                html += '<th>ID</th>';
                html += '<th>Nombre del Cargo</th>';
                html += '</tr></thead><tbody>';

                data.data.forEach(cargo => {
                    html += '<tr>';
                    html += `<td>${cargo.CargoId}</td>`;
                    html += `<td>${cargo.NombreCargo}</td>`;
                    html += '</tr>';
                });

                html += '</tbody></table>';
                document.getElementById('modalContenido').innerHTML = html;
            } else {
                mostrarError(data.message);
            }
        })
        .catch(error => {
            mostrarError('Error al cargar los cargos: ' + error);
        });
}

// ============================================
// VER LISTA DE TIPOS DE PENSIÓN
// ============================================
function verListaPensiones() {
    abrirModal('💰 Tipos de Pensión');

    fetch('/Contratos/ObtenerTiposPension')
        .then(response => response.json())
        .then(data => {
            ocultarLoading();
            if (data.success) {
                if (data.data.length === 0) {
                    document.getElementById('modalContenido').innerHTML = `
                        <div style="text-align: center; padding: 40px; color: #999;">
                            <p>📭 No hay tipos de pensión registrados</p>
                        </div>
                    `;
                    return;
                }

                let html = '<table class="data-table"><thead><tr>';
                html += '<th>ID</th>';
                html += '<th>Tipo de Pensión</th>';
                html += '</tr></thead><tbody>';

                data.data.forEach(pension => {
                    html += '<tr>';
                    html += `<td>${pension.TipoPensionId}</td>`;
                    html += `<td>${pension.NombreTipo}</td>`;
                    html += '</tr>';
                });

                html += '</tbody></table>';
                document.getElementById('modalContenido').innerHTML = html;
            } else {
                mostrarError(data.message);
            }
        })
        .catch(error => {
            mostrarError('Error al cargar los tipos de pensión: ' + error);
        });
}

// ============================================
// VER LISTA DE ESTADOS DE CONTRATO
// ============================================
function verListaEstados() {
    abrirModal('📊 Estados de Contrato');

    fetch('/Contratos/ObtenerEstadosContrato')
        .then(response => response.json())
        .then(data => {
            ocultarLoading();
            if (data.success) {
                if (data.data.length === 0) {
                    document.getElementById('modalContenido').innerHTML = `
                        <div style="text-align: center; padding: 40px; color: #999;">
                            <p>📭 No hay estados registrados</p>
                        </div>
                    `;
                    return;
                }

                let html = '<table class="data-table"><thead><tr>';
                html += '<th>ID</th>';
                html += '<th>Estado</th>';
                html += '<th>Descripción</th>';
                html += '</tr></thead><tbody>';

                data.data.forEach(estado => {
                    let badgeClass = 'badge-activo';
                    switch (estado.EstadoId) {
                        case 1: badgeClass = 'badge-activo'; break;
                        case 2: badgeClass = 'badge-finalizado'; break;
                        case 3: badgeClass = 'badge-suspendido'; break;
                        case 4: badgeClass = 'badge-inactivo'; break;
                    }

                    html += '<tr>';
                    html += `<td>${estado.EstadoId}</td>`;
                    html += `<td><span class="badge ${badgeClass}">${estado.NombreEstado}</span></td>`;
                    html += `<td>Estado ${estado.NombreEstado.toLowerCase()} del contrato</td>`;
                    html += '</tr>';
                });

                html += '</tbody></table>';
                document.getElementById('modalContenido').innerHTML = html;
            } else {
                mostrarError(data.message);
            }
        })
        .catch(error => {
            mostrarError('Error al cargar los estados: ' + error);
        });
}