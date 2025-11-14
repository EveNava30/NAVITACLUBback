using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces;

namespace ReservasNC.Api.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificationService _notification;

        public NotificacionesController(INotificationService notification)
        {
            _notification = notification;
        }

        [HttpPost("registrar-token")]
        public async Task<IActionResult> RegistrarToken(int idUsuario, string token)
        {
            await _notification.RegisterFcmTokenAsync(idUsuario, token);
            return Ok(new { mensaje = "Token registrado" });
        }
    }
}
