using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
    }
}
