using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ReservasNC.Api.Hubs
{
    public class ReservaHub : Hub
    {
        // Este método se ejecuta cuando un cliente se conecta al hub
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", "Conectado al servidor de reservas en tiempo real ✅");
        }

        // Método para enviar notificaciones a todos los clientes
        public async Task NotificarCambioReserva(string mensaje)
        {
            await Clients.All.SendAsync("ReservaActualizada", mensaje);
        }
    }
}
