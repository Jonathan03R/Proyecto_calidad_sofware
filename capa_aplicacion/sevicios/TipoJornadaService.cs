using capa_dominio;
using System.Collections.Generic;
using System;

namespace capa_aplicacion.sevicios
{
    public class TipoJornadaService
    {
        public List<TipoJornada> ObtenerTiposJornadas()
        {
            // Simulación de datos
            return new List<TipoJornada>
            {
                new TipoJornada
                {
                    TipoJornadaId = 1,
                    TipoJornadaNombre = "Tiempo Completo",
                    TipoJornadaDescripcion = "Jornada laboral de tiempo completo",
                    TipoJornadaEstado = 'A',
                    TipoJornadaFechaCreacion = DateTime.Now
                },
                new TipoJornada
                {
                    TipoJornadaId = 2,
                    TipoJornadaNombre = "Medio Tiempo",
                    TipoJornadaDescripcion = "Jornada laboral de medio tiempo",
                    TipoJornadaEstado = 'A',
                    TipoJornadaFechaCreacion = DateTime.Now
                }
            };
        }
    }
}