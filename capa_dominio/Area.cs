using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Area
    {
        // ======== Atributos privados ==========
        private int areaId;
        private string areaNombre;
        private string areaDescripcion;
        private Sede sede;
        private char areaEstado;
        private DateTime areaCreacion;

        // ======== Propiedades públicas ==========
        public int AreaId { get => areaId; set => areaId = value; }
        public string AreaNombre { get => areaNombre; set => areaNombre = value; }
        public string AreaDescripcion { get => areaDescripcion; set => areaDescripcion = value; }
        public Sede Sede { get => sede; set => sede = value; }
        public char AreaEstado { get => areaEstado; set => areaEstado = value; }
        public DateTime AreaCreacion { get => areaCreacion; set => areaCreacion = value; }

        // ======== Métodos de negocio ==========
        public bool EstaActiva()
        {
            return areaEstado == 'A' || areaEstado == 'a';
        }

        public void Activar()
        {
            areaEstado = 'A';
        }

        public void Desactivar()
        {
            areaEstado = 'I';
        }

        public string Resumen()
        {
            string estado = EstaActiva() ? "Activa" : "Inactiva";
            return $"{areaNombre} ({estado}) - {(sede != null ? sede.SedeNombre : "Sin sede asignada")}";
        }
    }
}