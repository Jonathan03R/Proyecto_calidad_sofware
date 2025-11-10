using System;
using System.Collections.Generic;
using System.Linq;
using capa_dominio;
using capa_dominio.dto;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;


namespace capa_aplicacion.servicios
{
    public class NominasServicios
    {
        private readonly AccesoSQLServer _conexion;
        private readonly NominasRepositorio _nominas;
        private readonly DetallesNominaRepositorio _detalleNomina;
        private readonly TrabajadoresRepositorio _trabajadores;
        private readonly HijosRepositorio _hijos;

        public NominasServicios()
        {
            _conexion = new AccesoSQLServer();
            _nominas = new NominasRepositorio(_conexion);
            _detalleNomina = new DetallesNominaRepositorio(_conexion);
            _trabajadores = new TrabajadoresRepositorio(_conexion);
            _hijos = new HijosRepositorio(_conexion);
        }

        public void ProcesarNominaPorPeriodo(
    int periodoId,
    List<ImpuestoRentaTramo> tramos,
    Parametro parametroEssalud,
    decimal valorUIT)
        {
            Console.WriteLine($"🔄 Iniciando ProcesarNominaPorPeriodo para período: {periodoId}");

            try
            {
                // 1. Obtener trabajadores
                Console.WriteLine("🔍 Obteniendo trabajadores...");
                var trabajadores = _trabajadores.ObtenerEmpleados();
                Console.WriteLine($"📊 Trabajadores obtenidos: {trabajadores?.Count ?? 0}");

                if (trabajadores == null || !trabajadores.Any())
                {
                    throw new Exception("No se encontraron trabajadores en la base de datos");
                }

                // 2. Verificar contratos
                int trabajadoresConContrato = 0;
                int trabajadoresSinContrato = 0;

                foreach (var trabajador in trabajadores)
                {
                    if (trabajador.Contrato != null)
                        trabajadoresConContrato++;
                    else
                        trabajadoresSinContrato++;
                }

                Console.WriteLine($"📝 Trabajadores con contrato: {trabajadoresConContrato}");
                Console.WriteLine($"⚠️ Trabajadores sin contrato: {trabajadoresSinContrato}");

                if (trabajadoresConContrato == 0)
                {
                    throw new Exception("No se encontraron trabajadores con contratos activos");
                }

                var nomina = new Nomina { Periodo = new Periodo { PeriodoId = periodoId } };

                _conexion.AbrirConexion();
                _conexion.IniciarTransaccion();

                try
                {
                    var nominaId = _nominas.IniciarProcesoPorPeriodo(periodoId, "Nómina generada automáticamente");
                    nomina.NominaId = nominaId;
                    nomina.NominaEstado = "Procesando";
                    nomina.Detalles = new List<DetalleNomina>();

                    Console.WriteLine($"🆕 Nómina creada con ID: {nominaId}");

                    int procesados = 0;
                    int omitidos = 0;

                    foreach (var trabajador in trabajadores)
                    {
                        try
                        {
                            Console.WriteLine($"👤 Procesando trabajador {trabajador.TrabajadorId}: {trabajador.Nombres}");

                            trabajador.Hijos = _hijos.ObtenerHijosPorTrabajador(trabajador.TrabajadorId);
                            Console.WriteLine($"👶 Hijos del trabajador: {trabajador.Hijos?.Count ?? 0}");

                            var contrato = trabajador.Contrato;
                            if (contrato == null)
                            {
                                Console.WriteLine($"⚠️ Trabajador {trabajador.TrabajadorId} sin contrato - omitiendo");
                                omitidos++;
                                continue;
                            }

                            Console.WriteLine($"💰 Contrato encontrado - Salario: {contrato.ContratoSalario}");

                            var detalle = new DetalleNomina
                            {
                                Contrato = contrato,
                                SueldoBasico = contrato.ContratoSalario
                            };

                            // Procesar cálculos
                            bool tieneAsignacion = trabajador.TieneDerechoAsignacionFamiliar();
                            Console.WriteLine($"🏠 Derecho a asignación familiar: {tieneAsignacion}");

                            detalle.CalculoAsignacionFamiliar(tieneAsignacion);
                            detalle.CalcularHorasExtras();
                            detalle.CalcularRemuneracionBruta();
                            detalle.CalcularSistemaPensiones();
                            detalle.CalcularAporteEssalud(parametroEssalud);
                            detalle.CalcularImpuestoRentaQuinta(tramos, valorUIT);
                            detalle.CalcularTotales();

                            var dto = new DetalleNominaDTO
                            {
                                NominaId = nominaId,
                                TrabajadorId = trabajador.TrabajadorId,
                                RemuneracionBruta = detalle.RemuneracionBruta,
                                SueldoBasico = detalle.SueldoBasico,
                                AsignacionFamiliar = detalle.AsignacionFamiliar,
                                HorasExtras = detalle.HorasExtras,
                                BonosRegulares = detalle.BonosRegulares,
                                OtrosIngresos = detalle.OtrosIngresos,
                                AporteEssalud = detalle.AporteEssalud,
                                AporteOnp = detalle.AporteONP,
                                DescuentoAfp = detalle.DescuentoAFP,
                                ImpuestoRentaMensual = detalle.ImpuestoRentaMensual,
                                TotalIngresos = detalle.TotalIngresos,
                                TotalDescuentos = detalle.TotalDescuentos,
                                NetoPagar = detalle.NetoPagar
                            };

                            _detalleNomina.InsertarDetalleNomina(dto);
                            nomina.Detalles.Add(detalle);
                            procesados++;

                            Console.WriteLine($"✅ Trabajador {trabajador.TrabajadorId} procesado - Neto: {detalle.NetoPagar}");

                        }
                        catch (Exception exTrabajador)
                        {
                            Console.WriteLine($"❌ Error procesando trabajador {trabajador.TrabajadorId}: {exTrabajador.Message}");
                            omitidos++;
                        }
                    }

                    Console.WriteLine($"📊 Resumen: {procesados} procesados, {omitidos} omitidos");

                    if (procesados == 0)
                    {
                        throw new Exception("No se pudo procesar ningún trabajador");
                    }

                    nomina.CalcularTotales();

                    _nominas.ActualizarTotales(
                        nomina.NominaId,
                        nomina.NominaTotalEmpleados,
                        nomina.NominaTotalBruto,
                        nomina.NominaTotalDescuentos,
                        nomina.NominaTotalNeto,
                        "Exitoso"
                    );

                    _conexion.TerminarTransaccion();
                    Console.WriteLine("✅ Nómina procesada exitosamente");
                }
                catch (Exception exProceso)
                {
                    _conexion.CancelarTransaccion();
                    if (nomina?.NominaId > 0)
                    {
                        _nominas.ActualizarEstado(nomina.NominaId, "Con Errores");
                    }
                    Console.WriteLine($"❌ Error en proceso de nómina: {exProceso.Message}");
                    throw;
                }
                finally
                {
                    _conexion.CerrarConexion();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error general en ProcesarNominaPorPeriodo: {ex.Message}");
                Console.WriteLine($"📝 StackTrace: {ex.StackTrace}");
                throw new Exception($"No se pudo consultar el(los) trabajador(es), intente nuevamente o consulte con el administrador. Detalle: {ex.Message}", ex);
            }

        }
    }
}