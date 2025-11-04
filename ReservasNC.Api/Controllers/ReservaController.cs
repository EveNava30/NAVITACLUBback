using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _service;

        public ReservaController(IReservaService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _service.GetReservasAsync();
            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _service.GetReservaByIdAsync(id);
            if (reserva == null) return NotFound();
            return Ok(reserva);
        }
        [HttpGet("fecha/{fecha}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            var reservas = await _service.GetReservasPorFechaAsync(fecha);
            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas para la fecha {fecha:yyyy-MM-dd}");

            return Ok(reservas);
        }

        [HttpGet("rango")]
        public async Task<IActionResult> GetByRangoFechas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha final.");

            var reservas = await _service.GetReservasPorRangoFechasAsync(fechaInicio, fechaFin);
            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas entre {fechaInicio:yyyy-MM-dd} y {fechaFin:yyyy-MM-dd}");

            return Ok(reservas);
        }


        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario)
        {
            var reservas = await _service.GetReservasPorUsuarioAsync(idUsuario);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas registradas para el usuario con ID {idUsuario}");

            return Ok(reservas);
        }
        [HttpGet("restaurante/{idRestaurante}")]
        public async Task<IActionResult> GetByRestaurante(int idRestaurante)
        {
            var reservas = await _service.GetReservasPorRestauranteAsync(idRestaurante);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas para el restaurante con ID {idRestaurante}");

            return Ok(reservas);
        }
        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var detalle = await _service.GetReservaDetalleAsync(id);
            if (detalle == null)
                return NotFound($"No se encontró detalle para la reserva con ID {id}");

            return Ok(detalle);
        }


        /*[HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Reserva reserva)
        {
            var created = await _service.AddReservaAsync(reserva);
            return CreatedAtAction(nameof(GetById), new { id = created.IdReserva }, created);
        }
        */
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ReservaCreateDto dto)
        {
            var created = await _service.AddReservaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.IdReserva }, created);
        }




        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Reserva reserva)
        {
            if (id != reserva.IdReserva)
                return BadRequest("El ID de la reserva no coincide.");

            await _service.UpdateReservaAsync(reserva);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteReservaAsync(id);
            return NoContent();
        }
    }
}
