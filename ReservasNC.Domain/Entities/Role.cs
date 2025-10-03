using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Entities
{
    public class Role
    {
        public int IdRole { get; set; }
        public string NombreRol { get; set; } = null!;
        public DateTime FechaUltimaModificacion { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
