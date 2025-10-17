using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecret = "EstaEsUnaClaveSuperSeguraDe32Caracteres34";
        private readonly int _jwtLifespan = 60; // duración en minutos

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ------------------------------
        // LOGIN + TOKEN JWT
        // ------------------------------
        public async Task<(User? user, string? token)> LoginUserAsync(string email, string contrasena)
        {
            var user = await _userRepository.LoginUserAsync(email, contrasena);
            if (user == null) return (null, null);

            var token = GenerateJwtToken(user);
            return (user, token);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("idUser", user.IdUser.ToString()),
                new Claim("nombre", user.Nombre),
                new Claim("idRole", user.IdRole.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtLifespan),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ------------------------------
        // CRUD Y CAMBIO DE CONTRASEÑA
        // ------------------------------
        public async Task<User> AddUserAsync(User user)
        {
            var newUserId = await _userRepository.AddUserAsync(user);
            user.IdUser = newUserId;
            return user;
        }

        public async Task<List<User>> GetUsersAsync() => await _userRepository.GetUsersAsync();

        public async Task<User?> GetByIdAsync(int idUser) => await _userRepository.GetByIdAsync(idUser);

        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateUserAsync(user);

        public async Task DeleteUserAsync(int idUser) => await _userRepository.DeleteUserAsync(idUser);

        public async Task ChangePasswordAsync(string email, string contrasenaActual, string nuevaContrasena) =>
            await _userRepository.ChangePasswordAsync(email, contrasenaActual, nuevaContrasena);
    }
}
