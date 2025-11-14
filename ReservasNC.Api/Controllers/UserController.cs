using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.DTOs;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Api.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar usuarios, autenticación y recuperación de contraseñas.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        #region == Inyección de dependencias ==

        private readonly IUserService _userService;

        /// <summary>
        /// Constructor que recibe el servicio de usuarios.
        /// </summary>
        /// <param name="userService">Servicio que maneja la lógica de negocio de usuarios.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region == CRUD DE USUARIOS ==

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Los datos del usuario no pueden ser nulos.");

            var createdUser = await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.IdUser }, createdUser);
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados.
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtiene un usuario por su identificador único.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound($"No se encontró un usuario con ID {id}.");

            return Ok(user);
        }

        /// <summary>
        /// Actualiza la información de un usuario existente.
        /// </summary>
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null)
                return BadRequest("Los datos del usuario no pueden ser nulos.");

            if (id != user.IdUser)
                return BadRequest("El ID del usuario no coincide con el de la ruta.");

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario existente por su ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        #endregion

        #region == AUTENTICACIÓN Y LOGIN ==

        /// <summary>
        /// Inicia sesión y genera un token JWT si las credenciales son válidas.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new { mensaje = "Correo y contraseña son obligatorios." });

            var (user, token) = await _userService.LoginUserAsync(request.Email.Trim(), request.Contrasena.Trim());

            if (user == null)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            return Ok(new
            {
                mensaje = "Login exitoso.",
                token,
                usuario = new
                {
                    id = user.IdUser,
                    nombre = user.Nombre,
                    email = user.Email,
                    rol = user.NombreRol // 👈 Aquí se incluye el nombre del rol (Administrador, Mesero, Cliente)
                }
            });

        }

        /// <summary>
        /// DTO usado para solicitudes de inicio de sesión.
        /// </summary>
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
        }

        /// <summary>
        /// Permite al usuario cambiar su contraseña actual por una nueva.
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromForm] string email,
            [FromForm] string contrasenaActual,
            [FromForm] string nuevaContrasena)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(contrasenaActual) ||
                string.IsNullOrWhiteSpace(nuevaContrasena))
                return BadRequest(new { mensaje = "Todos los campos son obligatorios." });

            await _userService.ChangePasswordAsync(email, contrasenaActual, nuevaContrasena);
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        #endregion

        #region == RECUPERACIÓN DE CONTRASEÑA ==

        /// <summary>
        /// Envía un correo con instrucciones para recuperar la contraseña.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email))
                return BadRequest(new { mensaje = "El correo electrónico es obligatorio." });

            await _userService.ForgotPasswordAsync(req.Email);
            return Ok(new { mensaje = "Si el correo existe, se enviaron las instrucciones." });
        }

        /// <summary>
        /// Restablece la contraseña de un usuario usando un token de recuperación.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            try
            {
                await _userService.ResetPasswordAsync(request.Token, request.NuevaContrasena);
                return Ok(new { mensaje = "La contraseña se ha restablecido correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// DTO usado para la solicitud de recuperación de contraseña.
        /// </summary>
        public class ForgotRequest
        {
            public string Email { get; set; } = string.Empty;
        }

        #endregion
    }
}
