
namespace ReservasNC.Domain.Interfaces.Repositories
{
    public interface IRestauranteRepository
    {
        Task<List<Restaurante>> GetRestaurantesAsync();
      //  Task<List<MesaDto>> GetMesasDisponiblesAsync(int idRestaurante, DateTime fecha);

        Task<int> CrearRestauranteAsync(Restaurante restaurante);
        Task<int> CrearMesaAsync(Mesa mesa);

        //Task<HorarioRestaurante?> GetHorarioAsync(int idRestaurante);
    }
}
