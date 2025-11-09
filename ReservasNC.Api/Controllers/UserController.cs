using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.DTOs;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ------------------------------------------------------------
        // 🧩 CRUD DE USUARIOS
        // ------------------------------------------------------------
        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            var createdUser = await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.IdUser }, createdUser);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.IdUser)
                return BadRequest("El ID del usuario no coincide con el de la ruta.");

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        // ------------------------------------------------------------
        // 🔐 LOGIN Y CAMBIO DE CONTRASEÑA
        // ------------------------------------------------------------
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
                mensaje = "Login exitoso",
                usuario = user,
                token
            });
        }

        // DTO para login
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] string email, [FromForm] string contrasenaActual, [FromForm] string nuevaContrasena)
        {
            await _userService.ChangePasswordAsync(email, contrasenaActual, nuevaContrasena);
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        // ------------------------------------------------------------
        // 📧 RECUPERACIÓN DE CONTRASEÑA
        // ------------------------------------------------------------
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotRequest req)
        {
            await _userService.ForgotPasswordAsync(req.Email);
            return Ok(new { mensaje = "Si el correo existe, se enviaron las instrucciones." });
        }

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

        // ------------------------------------------------------------
        // DTO PARA RECUPERACIÓN DE CONTRASEÑA
        // ------------------------------------------------------------
        public class ForgotRequest
        {
            public string Email { get; set; } = string.Empty;
        }
    }
}
