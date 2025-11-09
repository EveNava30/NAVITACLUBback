using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendResetEmailAsync(string emailDestino, string token);
    }
}