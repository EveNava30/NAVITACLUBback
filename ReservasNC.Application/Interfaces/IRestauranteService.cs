namespace ReservasNC.Application.Interfaces
{
    public interface IRestauranteService
    {
        Task<List<Restaurante>> GetRestaurantesAsync();
      //  Task<List<Mesa>> GetMesasDisponiblesAsync(int idRestaurante, DateTime fecha);
        Task<int> CrearRestauranteAsync(Restaurante restaurante);
        Task<int> CrearMesaAsync(Mesa mesa);

        // Task<HorarioRestaurante?> GetHorarioAsync(int idRestaurante);
    }
}
