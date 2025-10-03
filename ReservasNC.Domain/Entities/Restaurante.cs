using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Restaurante
    {
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Descripcion { get; set; }
        public int Capacidad { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
    }
    
}
