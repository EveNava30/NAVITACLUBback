using Microsoft.AspNetCore.Mvc;
using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Application.Services;
using ReservasNC.Domain.Entities;

namespace ReservasNC.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones relacionadas con las reservas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        #region == Inyección de dependencias ==

        private readonly IReservaService _service;

        /// <summary>
        /// Constructor que recibe el servicio de reservas.
        /// </summary>
        /// <param name="service">Servicio que maneja la lógica de negocio de reservas.</param>
        public ReservaController(IReservaService service)
        {
            _service = service;
        }

        #endregion

        #region == Métodos GET ==

        /// <summary>
        /// Obtiene todas las reservas registradas.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _service.GetReservasAsync();
            return Ok(reservas);
        }

        /// <summary>
        /// Obtiene una reserva por su identificador único.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _service.GetReservaByIdAsync(id);

            if (reserva == null)
                return NotFound($"No se encontró una reserva con ID {id}.");

            return Ok(reserva);
        }

        /// <summary>
        /// Obtiene las reservas correspondientes a una fecha específica.
        /// </summary>
        [HttpGet("fecha/{fecha:datetime}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            var reservas = await _service.GetReservasPorFechaAsync(fecha);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas para la fecha {fecha:yyyy-MM-dd}.");

            return Ok(reservas);
        }

        /// <summary>
        /// Obtiene las reservas dentro de un rango de fechas.
        /// </summary>
        [HttpGet("rango")]
        public async Task<IActionResult> GetByRangoFechas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha final.");

            var reservas = await _service.GetReservasPorRangoFechasAsync(fechaInicio, fechaFin);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas entre {fechaInicio:yyyy-MM-dd} y {fechaFin:yyyy-MM-dd}.");

            return Ok(reservas);
        }

        /// <summary>
        /// Obtiene todas las reservas de un usuario específico.
        /// </summary>
        [HttpGet("usuario/{idUsuario:int}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario)
        {
            var reservas = await _service.GetReservasPorUsuarioAsync(idUsuario);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas registradas para el usuario con ID {idUsuario}.");

            return Ok(reservas);
        }

        /// <summary>
        /// Obtiene todas las reservas de un restaurante específico.
        /// </summary>
        [HttpGet("restaurante/{idRestaurante:int}")]
        public async Task<IActionResult> GetByRestaurante(int idRestaurante)
        {
            var reservas = await _service.GetReservasPorRestauranteAsync(idRestaurante);

            if (reservas == null || reservas.Count == 0)
                return NotFound($"No hay reservas para el restaurante con ID {idRestaurante}.");

            return Ok(reservas);
        }

        /// <summary>
        /// Obtiene el detalle completo de una reserva por su ID.
        /// </summary>
        [HttpGet("detalle/{id:int}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var detalle = await _service.GetReservaDetalleAsync(id);

            if (detalle == null)
                return NotFound($"No se encontró detalle para la reserva con ID {id}.");

            return Ok(detalle);
        }

        #endregion

        #region == Métodos POST ==

        /// <summary>
        /// Crea una nueva reserva en el sistema.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ReservaCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Los datos de la reserva no pueden ser nulos.");

            var created = await _service.AddReservaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.IdReserva }, created);
        }

        /// <summary>
        /// Crea un nuevo estado de la reserva
        /// </summary>
        [HttpPost("{id}/estatus/{idEstatus}")]
        public async Task<IActionResult> CambiarEstatusReserva(int id, int idEstatus, [FromBody] string? comentario)
        {
            await _service.CambiarEstatusReservaAsync(id, idEstatus, comentario);
            return Ok(new { mensaje = "Estatus de la reserva actualizado correctamente." });
        }


        #endregion

        #region == Métodos PUT ==

        /// <summary>
        /// Actualiza los datos de una reserva existente.
        /// </summary>
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Reserva reserva)
        {
            if (reserva == null)
                return BadRequest("Los datos de la reserva no pueden ser nulos.");

            if (id != reserva.IdReserva)
                return BadRequest("El ID de la reserva no coincide con el proporcionado en la ruta.");

            await _service.UpdateReservaAsync(reserva);
            return NoContent();
        }

        #endregion

        #region == Métodos DELETE ==

        /// <summary>
        /// Elimina una reserva existente por su ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteReservaAsync(id);
            return NoContent();
        }

        #endregion
    }
}
