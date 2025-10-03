using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public User Usuario { get; set; } = null!;
        public int IdMesa { get; set; }
        public Mesa Mesa { get; set; } = null!;
        public string? CodigoQR { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public ICollection<ReservaEstatus> HistorialEstatus { get; set; } = new List<ReservaEstatus>();
    }
}
