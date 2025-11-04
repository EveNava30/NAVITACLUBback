using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class HistorialEstatusData
    {
        public int IdReservaEstatus { get; set; }
        public int IdEstatus { get; set; }
        public string NombreEstatus { get; set; } = "";
        public string Comentario { get; set; } = "";
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
