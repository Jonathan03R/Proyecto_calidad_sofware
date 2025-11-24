using System.Collections.Generic;

namespace capa_presentacion.Models.Menu
{
    public class MenuItem
    {
        public string Titulo { get; set; }
        public string Icono { get; set; }
        public string Controlador { get; set; }
        public string Accion { get; set; }
        public List<MenuItem> SubItems { get; set; } = new List<MenuItem>();
    }
}
