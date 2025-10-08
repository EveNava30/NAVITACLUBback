using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Estatus
    {
        public int IdEstatus { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaUltimaModificacion { get; set; }
        public ICollection<ReservaEstatus> ReservaEstatus { get; set; } = new List<ReservaEstatus>();
    }

} // Intento 2 Juan Angel 
