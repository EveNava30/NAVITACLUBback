using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReservasNC.Domain.Entities;

namespace ReservasNC.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService; // ✅ Inyección del servicio de correo

        private readonly string _jwtSecret = "EstaEsUnaClaveSuperSeguraDe32Caracteres34";
        private readonly int _jwtLifespan = 60; // duración en minutos

        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        // ============================================================
        // 🔐 LOGIN + TOKEN JWT
        // ============================================================
        public async Task<(User? user, string? token)> LoginUserAsync(string email, string contrasena)
        {
            // 1️⃣ Busca el usuario por email
            var user = await _userRepository.LoginUserAsync(email);
            if (user == null)
                return (null, null);

            // 2️⃣ Verifica la contraseña ingresada contra el hash almacenado
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(contrasena.Trim(), user.Contrasena);
            if (!passwordMatch)
                return (null, null);

            // 3️⃣ Si coincide, genera el JWT
            var token = GenerateJwtToken(user);
            return (user, token);
        }

        // ============================================================
        // 🎟️ GENERACIÓN DEL TOKEN JWT
        // ============================================================
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

        // ============================================================
        // 👤 CRUD Y CAMBIO DE CONTRASEÑA
        // ============================================================
        public async Task<User> AddUserAsync(User user)
        {
            // 🔒 Encriptar la contraseña antes de guardar
            user.Contrasena = BCrypt.Net.BCrypt.HashPassword(user.Contrasena);

            var newUserId = await _userRepository.AddUserAsync(user);
            user.IdUser = newUserId;
            return user;
        }

        public async Task<List<User>> GetUsersAsync() =>
            await _userRepository.GetUsersAsync();

        public async Task<User?> GetByIdAsync(int idUser) =>
            await _userRepository.GetByIdAsync(idUser);

        public async Task UpdateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Contrasena))
                user.Contrasena = BCrypt.Net.BCrypt.HashPassword(user.Contrasena);

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int idUser) =>
            await _userRepository.DeleteUserAsync(idUser);

        // ============================================================
        // 🔁 CAMBIO DE CONTRASEÑA (verifica hash actual)
        // ============================================================
        public async Task ChangePasswordAsync(string email, string contrasenaActual, string nuevaContrasena)
        {
            var user = await _userRepository.LoginUserAsync(email);
            if (user == null)
                throw new Exception("Correo incorrecto.");

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(contrasenaActual, user.Contrasena);
            if (!passwordMatch)
                throw new Exception("Contraseña actual incorrecta.");

            string hashedNueva = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            await _userRepository.ChangePasswordAsync(email, hashedNueva);
        }

        // ============================================================
        // 🧩 RECUPERAR CONTRASEÑA
        // ============================================================
        // -------- Recuperación --------
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return; // no informar para seguridad

            string token = Guid.NewGuid().ToString();
            DateTime expiracion = DateTime.UtcNow.AddHours(1);

            await _userRepository.SaveResetTokenAsync(email, token, expiracion);

            // Enviar correo
            await _emailService.SendResetEmailAsync(email, token);
        }

        public async Task ResetPasswordAsync(string token, string nuevaContrasena)
        {
            var user = await _userRepository.GetUserByResetTokenAsync(token);
            if (user == null || user.TokenExpira == null || user.TokenExpira < DateTime.UtcNow)
                throw new Exception("Token inválido o expirado.");

            // Hash de la contraseña
            string hashed = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena.Trim());

            await _userRepository.ResetPasswordAsync(token, hashed);
        }

    }
}
