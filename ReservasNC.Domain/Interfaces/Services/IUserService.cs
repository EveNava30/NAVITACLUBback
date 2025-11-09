using ReservasNC.Domain.Entities;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<List<User>> GetUsersAsync();
        Task<User?> GetByIdAsync(int idUser);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int idUser);

        //  LOGIN + JWT opcional
        Task<(User? user, string? token)> LoginUserAsync(string email, string contrasena);

        //  CAMBIO DE CONTRASEÑA
        Task ChangePasswordAsync(string email, string contrasenaActual, string nuevaContrasena);

        // RECUPERACIÓN DE CONTRASEÑA
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(string token, string nuevaContrasena);
    }
}
