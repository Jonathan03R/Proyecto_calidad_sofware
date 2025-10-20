using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio
{
    public class Cargo
    {
        
        private int cargoId;
        private string cargoNombre;
        private char cargoEstado;
        private DateTime cargoFechaCreacion;

        
        public int CargoId { get => cargoId; set => cargoId = value; }
        public string CargoNombre { get => cargoNombre; set => cargoNombre = value; }
        public char CargoEstado { get => cargoEstado; set => cargoEstado = value; }
        public DateTime CargoFechaCreacion { get => cargoFechaCreacion; set => cargoFechaCreacion = value; }

        
        public bool EstaActivo()
        {
            return cargoEstado == 'A';
        }

        public void Activar()
        {
            cargoEstado = 'A';
        }

        public void Desactivar()
        {
            cargoEstado = 'I';
        }

        public string Resumen()
        {
            return $"{cargoNombre} - Estado: {(EstaActivo() ? "Activo" : "Inactivo")}";
        }
    }
}
