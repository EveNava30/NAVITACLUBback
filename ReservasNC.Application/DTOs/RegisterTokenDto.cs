using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.DTOs
{
    public class RegisterTokenDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}