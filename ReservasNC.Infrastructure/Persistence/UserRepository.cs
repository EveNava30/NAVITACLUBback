using Microsoft.EntityFrameworkCore;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Infrastructure.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservasNC.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // ========== CRUD BÁSICO ==========

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

        public async Task<User?> GetByIdAsync(int idUser)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC GetUserById {idUser}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC UpdateUser {user.IdUser}, {user.Nombre}, {user.Email}, {user.Telefono}, {user.IdRole}");
        }

        public async Task DeleteUserAsync(int idUser)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC DeleteUser {idUser}");
        }

        // ========== LOGIN ==========

        public async Task<User?> LoginUserAsync(string email)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC LoginUser {email}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        // ========== CAMBIO DE CONTRASEÑA ==========

        public async Task ChangePasswordAsync(string email, string nuevaContrasena)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC ChangePassword {email}, {nuevaContrasena}");
        }

        // ========== RECUPERACIÓN DE CONTRASEÑA ==========
        public async Task SaveResetTokenAsync(string email, string token, DateTime expiracion)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Usuario SET ResetToken = {token}, TokenExpira = {expiracion} WHERE Email = {email}");
        }

        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            var data = await _context.Users.FromSqlInterpolated(
                $"SELECT * FROM Usuario WHERE ResetToken = {token}").ToListAsync();
            return data.FirstOrDefault();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var data = await _context.Users.FromSqlInterpolated(
                $"SELECT * FROM Usuario WHERE Email = {email}").ToListAsync();
            return data.FirstOrDefault();
        }

        // Si usas SP ResetPassword, puedes llamar a él; aquí ejemplo directo:
        public async Task<bool> ResetPasswordAsync(string token, string hashedPassword)
        {
            var user = await GetUserByResetTokenAsync(token);
            if (user == null) return false;

            // NO hacer hash aquí, ya viene hasheada desde UserService
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE Usuario 
        SET Contrasena = {hashedPassword}, 
            ResetToken = NULL, 
            TokenExpira = NULL,
            FechaUltimaModificacion = GETDATE()
        WHERE IdUser = {user.IdUser}");

            return true;
        }
    }
}

