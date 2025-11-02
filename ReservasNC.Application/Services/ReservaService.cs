using Microsoft.AspNetCore.SignalR;
using ReservasNC.Api.Hubs;
using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Domain.Interfaces.Services;

namespace ReservasNC.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repo;
        private readonly IHubContext<ReservaHub> _hubContext;

        public ReservaService(IReservaRepository repo, IHubContext<ReservaHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task<List<Reserva>> GetReservasAsync() =>
            await _repo.GetReservasAsync();

        public async Task<Reserva?> GetReservaByIdAsync(int id) =>
            await _repo.GetReservaByIdAsync(id);

        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha) =>
           await _repo.GetReservasPorFechaAsync(fecha);

        public async Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin) =>
    await _repo.GetReservasPorRangoFechasAsync(fechaInicio, fechaFin);

        public async Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario) =>
            await _repo.GetReservasPorUsuarioAsync(idUsuario);

        public async Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante)
        {
            return await _repo.GetReservasPorRestauranteAsync(idRestaurante);
        }
        public async Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id)
        {
            var data = await _repo.GetReservaDetalleAsync(id);
            if (data == null) return null;

            return new ReservaDetalleDto
            {
                IdReserva = data.IdReserva,
                IdUsuario = data.IdUsuario,
                NombreUsuario = data.NombreUsuario,
                IdMesa = data.IdMesa,
                NumeroMesa = data.NumeroMesa,
                NombreRestaurante = data.NombreRestaurante,
                CodigoQR = data.CodigoQR,
                FechaHora = data.FechaHora,
                Historial = data.Historial.Select(h => new HistorialEstatusDto
                {
                    IdReservaEstatus = h.IdReservaEstatus,
                    IdEstatus = h.IdEstatus,
                    NombreEstatus = h.NombreEstatus,
                    Comentario = h.Comentario,
                    FechaUltimaModificacion = h.FechaUltimaModificacion
                }).ToList()
            };
        }



        public async Task<Reserva> AddReservaAsync(ReservaCreateDto dto)
        {
            var reserva = new Reserva
            {
                IdUsuario = dto.IdUsuario,
                IdMesa = dto.IdMesa,
                CodigoQR = dto.CodigoQR,
                FechaHora = dto.FechaHora,
                FechaUltimaModificacion = DateTime.Now,
                HistorialEstatus = dto.HistorialEstatus
                    .Select(e => new ReservaEstatus
                    {
                        IdEstatus = e.IdEstatus,
                        Comentario = e.Comentario,
                        FechaUltimaModificacion = DateTime.Now
                    }).ToList()
            };

            // Aquí llamas al repositorio
            var id = await _repo.AddReservaAsync(reserva);
            reserva.IdReserva = id;

            await _hubContext.Clients.All.SendAsync("ReservaActualizada", $"Nueva reserva creada con ID: {id}");

            return reserva;
        }




        /*public async Task<Reserva> AddReservaAsync(Reserva reserva)
        {
            var id = await _repo.AddReservaAsync(reserva);
            reserva.IdReserva = id;

            // Notificar a todos los clientes conectados
            await _hubContext.Clients.All.SendAsync("ReservaActualizada", $"Nueva reserva creada con ID: {id}");

            return reserva;
        }*/



        public async Task UpdateReservaAsync(Reserva reserva)
        {
            await _repo.UpdateReservaAsync(reserva);
            await _hubContext.Clients.All.SendAsync("ReservaActualizada", $"Reserva {reserva.IdReserva} actualizada");
        }

        public async Task DeleteReservaAsync(int id)
        {
            await _repo.DeleteReservaAsync(id);
            await _hubContext.Clients.All.SendAsync("ReservaActualizada", $"Reserva {id} eliminada");
        }
    }
}
