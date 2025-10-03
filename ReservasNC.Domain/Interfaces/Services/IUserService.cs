using ReservasNC.Domain.Entities;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(string nombre, string email, string password, int idRole);
        Task<User?> LoginAsync(string email, string password);
    }
}
