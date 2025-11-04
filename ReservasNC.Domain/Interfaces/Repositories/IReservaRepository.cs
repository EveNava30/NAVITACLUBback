namespace ReservasNC.Domain.Interfaces.Repositories
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> GetReservasAsync();
        Task<Reserva?> GetReservaByIdAsync(int id);
        //Task<int> AddReservaAsync(Reserva reserva);
        Task UpdateReservaAsync(Reserva reserva);
        Task DeleteReservaAsync(int id);
        Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha);
        Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);

        Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario);
        Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante);
        Task<int> AddReservaAsync(Reserva reserva);
        Task<ReservaDetalleData?> GetReservaDetalleAsync(int id);

    }
}
