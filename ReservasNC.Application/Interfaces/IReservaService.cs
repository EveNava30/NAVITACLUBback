using ReservasNC.Application.DTOs;

namespace ReservasNC.Application.Interfaces.Services
{
    /// <summary>
    /// Define las operaciones del servicio relacionadas con la gestión de reservas.
    /// </summary>
    public interface IReservaService
    {
        #region == Consultas (GET) ==

        /// <summary>
        /// Obtiene todas las reservas registradas.
        /// </summary>
        Task<List<Reserva>> GetReservasAsync();

        /// <summary>
        /// Obtiene una reserva por su identificador único.
        /// </summary>
        Task<Reserva?> GetReservaByIdAsync(int id);

        /// <summary>
        /// Obtiene las reservas realizadas en una fecha específica.
        /// </summary>
        Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha);

        /// <summary>
        /// Obtiene las reservas realizadas dentro de un rango de fechas.
        /// </summary>
        Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene las reservas asociadas a un restaurante.
        /// </summary>
        Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante);

        /// <summary>
        /// Obtiene las reservas realizadas por un usuario.
        /// </summary>
        Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario);

        /// <summary>
        /// Obtiene el detalle completo de una reserva.
        /// </summary>
        Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id);

        #endregion

        #region == Comandos (POST, PUT, DELETE) ==

        /// <summary>
        /// Agrega una nueva reserva al sistema.
        /// </summary>
        Task<Reserva> AddReservaAsync(ReservaCreateDto reserva);

        /// <summary>
        /// Actualiza los datos de una reserva existente.
        /// </summary>
        Task UpdateReservaAsync(Reserva reserva);

        /// <summary>
        /// Elimina una reserva por su ID.
        /// </summary>
        Task DeleteReservaAsync(int id);

        Task CambiarEstatusReservaAsync(int idReserva, int idEstatus, string? comentario = null);

        #endregion
    }
}
