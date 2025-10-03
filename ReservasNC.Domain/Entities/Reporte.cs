using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Reporte
    {
        public int IdReporte { get; set; }
        public int IdUsuario { get; set; }
        public User Usuario { get; set; } = null!;
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
