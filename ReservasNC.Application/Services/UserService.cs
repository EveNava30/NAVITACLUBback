using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReservasNC.Domain.Entities;

namespace ReservasNC.Application.Services
{
    /// <summary>
    /// Servicio de usuarios: gestiona autenticación JWT, CRUD y recuperación de contraseñas.
    /// </summary>
    public class UserService : IUserService
    {
        #region 🔧 Dependencias e inyección de servicios
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        private readonly string _jwtSecret = "EstaEsUnaClaveSuperSeguraDe32Caracteres34"; // ⚠️ Mover a configuración segura
        private readonly int _jwtLifespan = 60; // Duración del token (minutos)
        #endregion

        #region 🧩 Constructor
        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        #endregion

        #region 🔐 LOGIN + TOKEN JWT
        /// <summary>
        /// Valida las credenciales del usuario y genera un JWT si son correctas.
        /// </summary>
        public async Task<(User? user, string? token)> LoginUserAsync(string email, string contrasena)
        {
            var user = await _userRepository.LoginUserAsync(email);
            if (user == null)
                return (null, null);

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(contrasena.Trim(), user.Contrasena);
            if (!passwordMatch)
                return (null, null);

            var token = GenerateJwtToken(user);
            return (user, token);
        }

        /// <summary>
        /// Genera el token JWT firmado con los claims del usuario.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
        new Claim(ClaimTypes.Name, user.Nombre),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.NombreRol ?? "Cliente") // 👈 Aquí va el rol por nombre
    };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(_jwtLifespan),
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        #region 👤 CRUD DE USUARIOS
        public async Task<User> AddUserAsync(User user)
        {
            // 🔒 Encriptar contraseña antes de guardar
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
        #endregion

        #region 🔁 CAMBIO DE CONTRASEÑA
        /// <summary>
        /// Cambia la contraseña de un usuario verificando la actual mediante hash.
        /// </summary>
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
        #endregion

        #region 🧩 RECUPERAR CONTRASEÑA
        /// <summary>
        /// Inicia el proceso de recuperación de contraseña enviando un token por correo.
        /// </summary>
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return; // No informar por seguridad

            string token = Guid.NewGuid().ToString();
            DateTime expiracion = DateTime.UtcNow.AddHours(1);

            await _userRepository.SaveResetTokenAsync(email, token, expiracion);
            await _emailService.SendResetEmailAsync(email, token);
        }

        /// <summary>
        /// Restablece la contraseña de un usuario validando el token recibido.
        /// </summary>
        public async Task ResetPasswordAsync(string token, string nuevaContrasena)
        {
            var user = await _userRepository.GetUserByResetTokenAsync(token);
            if (user == null || user.TokenExpira == null || user.TokenExpira < DateTime.UtcNow)
                throw new Exception("Token inválido o expirado.");

            string hashed = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena.Trim());
            await _userRepository.ResetPasswordAsync(token, hashed);
        }
        #endregion
    }
}
