using System;
using System.Collections.Generic;
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
            var trabajadores = _trabajadores.ObtenerEmpleados();
            var nomina = new Nomina { Periodo = new Periodo { PeriodoId = periodoId } };

            _conexion.AbrirConexion();
            _conexion.IniciarTransaccion();

            try
            {
                var nominaId = _nominas.IniciarProcesoPorPeriodo(periodoId, "Nómina generada automáticamente");
                nomina.NominaId = nominaId;
                nomina.NominaEstado = "Procesando";
                nomina.Detalles = new List<DetalleNomina>();

                foreach (var trabajador in trabajadores)
                {
                    trabajador.Hijos = _hijos.ObtenerHijosPorTrabajador(trabajador.TrabajadorId);
                    var contrato = trabajador.Contrato;
                    if (contrato == null)
                        continue;

                    var detalle = new DetalleNomina
                    {
                        Contrato = contrato,
                        SueldoBasico = contrato.ContratoSalario
                    };

                    detalle.CalculoAsignacionFamiliar(trabajador.TieneDerechoAsignacionFamiliar());
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
            }
            catch (Exception ex)
            {
                _conexion.CancelarTransaccion();
                _nominas.ActualizarEstado(nomina.NominaId, "Con Errores");
                Console.WriteLine($"Error procesando nómina: {ex.Message}");
                throw;
            }
            finally
            {
                _conexion.CerrarConexion();
            }
        }
    }
}