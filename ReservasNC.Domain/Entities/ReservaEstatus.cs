using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class ReservaEstatus
    {
        public int IdReservaEstatus { get; set; }
        public int IdReserva { get; set; }
        public Reserva Reserva { get; set; } = null!;
        public int IdEstatus { get; set; }
        public Estatus Estatus { get; set; } = null!;
        public string? Comentario { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
