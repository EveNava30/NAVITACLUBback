using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasNC.Infrastructure.DataContexts;
using ReservasNC.Domain.Entities;
using ReservasNC.Application.DTOs.Usuario;

namespace ReservasNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Registrar usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var role = await _context.Roles.FindAsync(dto.IdRole);
            if (role == null)
                return BadRequest("Rol no válido.");

            var user = new User
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Contrasena = dto.Contrasena, // ⚠️ En producción: usa hash
                IdRole = dto.IdRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Retornar solo un DTO seguro
            var result = new
            {
                IdUser = user.IdUser,
                Nombre = user.Nombre,
                Email = user.Email,
                Telefono = user.Telefono,
                Rol = role.NombreRol
            };

            return Ok(new { message = "Usuario registrado correctamente", user = result });
        }


        // Login con validación de rol
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Contrasena == dto.Contrasena);

            if (user == null)
                return Unauthorized("Credenciales incorrectas.");

            // Crear DTO seguro para la respuesta
            var result = new
            {
                IdUser = user.IdUser,
                Nombre = user.Nombre,
                Email = user.Email,
                Telefono = user.Telefono,
                Rol = user.Role.NombreRol
            };

            return Ok(new { message = "Login exitoso", user = result });
        }

    }
}
