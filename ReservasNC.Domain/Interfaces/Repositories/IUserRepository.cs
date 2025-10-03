using ReservasNC.Domain.Entities;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
    }
}
