using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class HistorialEstatusDto
    {
        public int IdReservaEstatus { get; set; }
        public int IdEstatus { get; set; }
        public string NombreEstatus { get; set; } = null!;
        public string Comentario { get; set; } = null!;
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
