using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class ReservaCreateDto
    {
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public string? CodigoQR { get; set; }
        public DateTime FechaHora { get; set; }

        public List<ReservaEstatusCreateDto> HistorialEstatus { get; set; } = new();
    }
}
