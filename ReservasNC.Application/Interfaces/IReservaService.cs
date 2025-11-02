using ReservasNC.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Application.Interfaces.Services
{
    public interface IReservaService
    {
        Task<List<Reserva>> GetReservasAsync();
        Task<Reserva?> GetReservaByIdAsync(int id);
       // Task<Reserva> AddReservaAsync(Reserva reserva);
        Task UpdateReservaAsync(Reserva reserva);
        Task DeleteReservaAsync(int id);
        Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha);
        Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante);
        Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario);
        Task<Reserva> AddReservaAsync(ReservaCreateDto reserva);
        Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id);

    }
}
