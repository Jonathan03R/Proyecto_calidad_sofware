using capa_dominio;
using capa_persistencia.modulo_base;
using capa_persistencia.modulo_principal;
using System;
using System.Collections.Generic;

namespace capa_aplicacion.sevicios
{
    public class PeriodoService
    {
        private readonly PeriodosRepositorio periodosRepositorio;
        private readonly AccesoSQLServer conexion;

        public PeriodoService()
        {
            conexion = new AccesoSQLServer();
            periodosRepositorio = new PeriodosRepositorio(conexion);
        }

        // ================================
        // LISTAR PERIODOS (FILTROS OPCIONALES)
        // ================================
        public List<Periodo> ListarPeriodos(int? periodoId = null, string periodoNombre = null)
        {
            try
            {
                conexion.AbrirConexion();
                return periodosRepositorio.ListarTodosPeriodos(periodoId, periodoNombre);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        // ================================
        // LISTAR PENDIENTES
        // ================================
        public List<Periodo> ListarAbiertos()
        {
            try
            {
                conexion.AbrirConexion();
                return periodosRepositorio.ListarPeriodosAbiertos();
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        // ================================
        // LISTAR PROCESADOS
        // ================================
        public List<Periodo> ListarProcesados()
        {
            try
            {
                conexion.AbrirConexion();
                return periodosRepositorio.ListarPeriodosProcesados();
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        // ================================
        // LISTAR ANULADOS
        // ================================
        public List<Periodo> ListarPendientes()
        {
            try
            {
                conexion.AbrirConexion();
                return periodosRepositorio.ListarPeriodosPendientes();
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
       
    }
}
