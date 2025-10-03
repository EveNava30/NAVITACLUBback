using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace ReservasNC.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUserAsync(string nombre, string email, string password, int idRole)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("El correo ya está registrado");

            var hashedPassword = HashPassword(password);

            var user = new User
            {
                Nombre = nombre,
                Email = email,
                Contrasena = hashedPassword,
                IdRole = idRole,
                FechaUltimaModificacion = DateTime.UtcNow
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return VerifyPassword(password, user.Contrasena) ? user : null;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }
    }
}
