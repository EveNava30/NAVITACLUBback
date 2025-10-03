using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Mesa
    {
        public int IdMesa { get; set; }
        public int NumeroMesa { get; set; }
        public int Capacidad { get; set; }
        public int IdRestaurante { get; set; }
        public Restaurante Restaurante { get; set; } = null!;
        public DateTime FechaUltimaModificacion { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
