using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.Services;
using ReservasNC.Domain.Entities;

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

        [HttpPost("add")]
        public async Task<ActionResult<User>> AddUser(User user)
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(string email, string contrasena)
        {
            var (user, token) = await _userService.LoginUserAsync(email, contrasena);

            if (user == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            return Ok(new
            {
                mensaje = "Login exitoso",
                usuario = user,
                token
            });
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string email, string contrasenaActual, string nuevaContrasena)
        {
            await _userService.ChangePasswordAsync(email, contrasenaActual, nuevaContrasena);
            return NoContent();
        }
    }
}