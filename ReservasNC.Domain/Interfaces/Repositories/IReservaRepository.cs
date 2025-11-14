namespace ReservasNC.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para operaciones de acceso a datos de reservas.
    /// </summary>
    public interface IReservaRepository
    {
        #region 🔍 Consultas generales
        Task<List<Reserva>> GetReservasAsync();
        Task<Reserva?> GetReservaByIdAsync(int id);
        Task<ReservaDetalleData?> GetReservaDetalleAsync(int id);
        #endregion

        #region 📅 Consultas por fecha
        Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha);
        Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        #endregion

        #region 👤 Consultas por usuario / restaurante
        Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario);
        Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante);
        #endregion

        #region 🧩 Operaciones CRUD
        Task<int> AddReservaAsync(Reserva reserva);
        Task UpdateReservaAsync(Reserva reserva);
        Task DeleteReservaAsync(int id);
        Task AddReservaEstatusAsync(int idReserva, int idEstatus, string? comentario);

        #endregion
    }
}
