namespace ReservasNC.Application.Interfaces
{
    public interface INotificationService
    {
        Task RegisterFcmTokenAsync(int idUsuario, string token);
        Task SendPushToUserAsync(int idUsuario, string titulo, string mensaje);
    }

}
