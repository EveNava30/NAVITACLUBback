using Microsoft.EntityFrameworkCore;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Infrastructure.DataContexts;

namespace ReservasNC.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddUserAsync(User user)
        {
            var result = await _context.Users
                .FromSqlInterpolated($"EXEC AddUser {user.Nombre}, {user.Email}, {user.Contrasena}, {user.Telefono}, {user.IdRole}")
                .ToListAsync();

            return result.FirstOrDefault()?.IdUser ?? 0;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users
                .FromSqlRaw("EXEC GetUsers")
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC GetUserById {id}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC UpdateUser {user.IdUser}, {user.Nombre}, {user.Email}, {user.Telefono}, {user.IdRole}");
        }

        public async Task DeleteUserAsync(int id)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC DeleteUser {id}");
        }

        public async Task<User?> LoginUserAsync(string email, string contrasena)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC LoginUser {email}, {contrasena}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        public async Task ChangePasswordAsync(string email, string contrasenaActual, string nuevaContrasena)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC ChangePassword {email}, {contrasenaActual}, {nuevaContrasena}");
        }
    }
}

