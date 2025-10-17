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

        //  Login con token
        Task<(User? user, string? token)> LoginUserAsync(string email, string contrasena);

        Task ChangePasswordAsync(string email, string contrasenaActual, string nuevaContrasena);
    }
}
