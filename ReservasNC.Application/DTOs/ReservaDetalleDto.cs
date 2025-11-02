using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class ReservaDetalleDto
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int IdMesa { get; set; }
        public string NumeroMesa { get; set; }
        public string NombreRestaurante { get; set; }
        public string CodigoQR { get; set; }
        public DateTime FechaHora { get; set; }
        public List<HistorialEstatusDto> Historial { get; set; } = new List<HistorialEstatusDto>();
    }
}
