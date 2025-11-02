using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class ReservaEstatusCreateDto
    {
        public int IdEstatus { get; set; }
        public string? Comentario { get; set; }
    }
}
