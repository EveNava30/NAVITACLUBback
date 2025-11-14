using ReservasNC.Application.DTOs;

namespace ReservasNC.Infrastructure.Persistence
{
    public class RestauranteRepository : IRestauranteRepository
    {
        private readonly AppDbContext _context;

        public RestauranteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Restaurante>> GetRestaurantesAsync()
        {
            return await _context.Set<Restaurante>()
                .FromSqlRaw("EXEC GetRestaurantes")
                .ToListAsync();
        }

        public async Task<List<MesaDto>> GetMesasDisponiblesAsync(int idRestaurante, DateTime fecha)
{
    return await _context.MesasDisponibles
        .FromSqlInterpolated($"EXEC GetMesasDisponibles {idRestaurante}, {fecha}")
        .ToListAsync();
}

       /* public async Task<List<Mesa>> GetMesasDisponiblesAsync(
    int idRestaurante,
    DateTime fecha,
    TimeSpan horaInicio,
    int duracion)
        {
            return await _context.Set<Mesa>()
                .FromSqlInterpolated($@"
            EXEC GetMesasDisponibles 
                @IdRestaurante = {idRestaurante}, 
                @Fecha = {fecha}, 
                @HoraInicio = {horaInicio}, 
                @Duracion = {duracion}")
                .ToListAsync();
        }
       */


        public async Task<int> CrearRestauranteAsync(Restaurante restaurante)
        {
            var id = await _context.Set<Restaurante>()
                .FromSqlInterpolated($@"
            EXEC CrearRestaurante 
                @Nombre = {restaurante.Nombre}, 
                @Direccion = {restaurante.Direccion}, 
                @Telefono = {restaurante.Telefono}, 
                @Descripcion = {restaurante.Descripcion}, 
                @Capacidad = {restaurante.Capacidad}")
                .Select(r => r.IdRestaurante)
                .FirstOrDefaultAsync();

            return id;
        }
        // ✅ Crear una nueva mesa
        public async Task<int> CrearMesaAsync(Mesa mesa)
        {
            var id = await _context.Set<Mesa>()
                .FromSqlInterpolated($@"
            EXEC CrearMesa
                @IdRestaurante = {mesa.IdRestaurante},
                @NumeroMesa = {mesa.NumeroMesa},
                @Capacidad = {mesa.Capacidad}")
                .Select(m => m.IdMesa)
                .FirstOrDefaultAsync();

            return id;
        }





        /*public async Task<HorarioRestaurante?> GetHorarioAsync(int idRestaurante)
         {
             var data = await _context.Set<HorarioRestaurante>()
                 .FromSqlInterpolated($"EXEC GetHorariosRestaurante {idRestaurante}")
                 .ToListAsync();

             return data.FirstOrDefault();
         }
        */
    }
}
