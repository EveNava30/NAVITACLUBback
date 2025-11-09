using ReservasNC.Domain.Entities;
using System.Threading.Tasks; 
namespace ReservasNC.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        // ------------------------------
        // CRUD DE USUARIOS
        // ------------------------------
        Task<int> AddUserAsync(User user);
        Task<List<User>> GetUsersAsync();
        Task<User?> GetByIdAsync(int idUser);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int idUser);

        // ------------------------------
        // LOGIN (obtiene usuario por email)
        // ------------------------------
        Task<User?> LoginUserAsync(string email);

        // ------------------------------
        // CAMBIO DE CONTRASEÑA
        // ------------------------------
        Task ChangePasswordAsync(string email, string nuevaContrasena);

        // ------------------------------
        // RECUPERACIÓN DE CONTRASEÑA (RESET PASSWORD)
        // ------------------------------
        Task SaveResetTokenAsync(string email, string token, DateTime expiracion);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }

}
