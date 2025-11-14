using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces;

namespace ReservasNC.Application.Services
{
    public class RestauranteService : IRestauranteService
    {
        private readonly IRestauranteRepository _repo;

        public RestauranteService(IRestauranteRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Restaurante>> GetRestaurantesAsync() =>
            await _repo.GetRestaurantesAsync();

       /* public async Task<List<MesaDto>> GetMesasDisponiblesAsync(int idRestaurante, DateTime fecha) =>
            await _repo.GetMesasDisponiblesAsync(idRestaurante, fecha);*/

       /* public async Task<List<Mesa>> GetMesasDisponiblesAsync(
    int idRestaurante,
    DateTime fecha,
    TimeSpan horaInicio,
    int duracion)
        {
            return await _repo.GetMesasDisponiblesAsync(idRestaurante, fecha, horaInicio, duracion);
        }*/


        public Task<int> CrearRestauranteAsync(Restaurante restaurante)
            => _repo.CrearRestauranteAsync(restaurante);

        public Task<int> CrearMesaAsync(Mesa mesa)
            => _repo.CrearMesaAsync(mesa);

        /* public async Task<HorarioRestaurante?> GetHorarioAsync(int idRestaurante) =>
             await _repo.GetHorarioAsync(idRestaurante);
        */
    }
}
