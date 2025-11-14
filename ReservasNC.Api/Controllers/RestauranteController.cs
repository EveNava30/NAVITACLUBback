using ReservasNC.Application.Interfaces;
using ReservasNC.Domain.Entities;

namespace ReservasNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestauranteController : ControllerBase
    {
        private readonly IRestauranteService _service;

        public RestauranteController(IRestauranteService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRestaurantes()
        {
            var data = await _service.GetRestaurantesAsync();
            return Ok(data);
        }

       /* [HttpGet("disponibles")]
        public async Task<IActionResult> GetMesasDisponibles(int idRestaurante, DateTime fecha)
        {
            var mesas = await _service.GetMesasDisponiblesAsync(idRestaurante, fecha);
            return Ok(mesas);
        }*/
        /*[HttpGet("disponibles")]
        public async Task<IActionResult> GetMesasDisponibles(
    int idRestaurante,
    DateTime fecha,
    TimeSpan horaInicio,   // nueva hora de inicio
    int duracion)          // duración en minutos
        {
            var mesas = await _service.GetMesasDisponiblesAsync(idRestaurante, fecha, horaInicio, duracion);
            return Ok(mesas);
        }*/


        // ✅ Crear un restaurante
        [HttpPost("crear")]
        public async Task<IActionResult> CrearRestaurante([FromBody] Restaurante restaurante)
        {
            if (restaurante == null)
                return BadRequest("Datos inválidos.");

            var id = await _service.CrearRestauranteAsync(restaurante);
            return Ok(new { message = "Restaurante creado con éxito", id });
        }

        // ✅ Crear una mesa
        [HttpPost("mesa")]
        public async Task<IActionResult> CrearMesa([FromBody] Mesa mesa)
        {
            if (mesa == null)
                return BadRequest("Datos inválidos.");

            var id = await _service.CrearMesaAsync(mesa);
            return Ok(new { message = "Mesa creada con éxito", id });
        }
    }


    /* [HttpGet("horario")]
     public async Task<IActionResult> GetHorario(int idRestaurante)
     {
         var horario = await _service.GetHorarioAsync(idRestaurante);
         return Ok(horario);
     }*/
}

