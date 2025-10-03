using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs.Usuario
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Contrasena { get; set; }

    }
}
