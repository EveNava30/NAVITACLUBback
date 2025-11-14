using Microsoft.AspNetCore.SignalR;
using QRCoder;
using ReservasNC.Api.Hubs;
using ReservasNC.Application.DTOs;
using ReservasNC.Application.Interfaces;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Domain.Interfaces.Repositories;
using System.Drawing.Imaging;

namespace ReservasNC.Application.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio relacionada con las reservas.
    /// </summary>
    public class ReservaService : IReservaService
    {
        #region == Dependencias ==

        private readonly IReservaRepository _repo;
        private readonly IHubContext<ReservaHub> _hubContext;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Constructor que inyecta las dependencias necesarias.
        /// </summary>
        public ReservaService(
            IReservaRepository repo,
            IHubContext<ReservaHub> hubContext,
            INotificationService notificationService)
        {
            _repo = repo;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        #endregion

        #region == Consultas (GET) ==

        /// <summary>
        /// Obtiene todas las reservas registradas.
        /// </summary>
        public async Task<List<Reserva>> GetReservasAsync() =>
            await _repo.GetReservasAsync();

        /// <summary>
        /// Obtiene una reserva por su identificador.
        /// </summary>
        public async Task<Reserva?> GetReservaByIdAsync(int id) =>
            await _repo.GetReservaByIdAsync(id);

        /// <summary>
        /// Obtiene todas las reservas realizadas en una fecha específica.
        /// </summary>
        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha) =>
            await _repo.GetReservasPorFechaAsync(fecha);

        /// <summary>
        /// Obtiene las reservas dentro de un rango de fechas.
        /// </summary>
        public async Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin) =>
            await _repo.GetReservasPorRangoFechasAsync(fechaInicio, fechaFin);

        /// <summary>
        /// Obtiene todas las reservas realizadas por un usuario.
        /// </summary>
        public async Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario) =>
            await _repo.GetReservasPorUsuarioAsync(idUsuario);

        /// <summary>
        /// Obtiene las reservas de un restaurante específico.
        /// </summary>
        public async Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante) =>
            await _repo.GetReservasPorRestauranteAsync(idRestaurante);

        /// <summary>
        /// Obtiene el detalle completo de una reserva.
        /// </summary>
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

        #endregion

        #region == Creación (POST) ==

        /// <summary>
        /// Crea una nueva reserva en el sistema, envía notificación y actualiza clientes mediante SignalR.
        /// </summary>
        public async Task<Reserva> AddReservaAsync(ReservaCreateDto dto)
        {
            var reserva = new Reserva
            {
                IdUsuario = dto.IdUsuario,
                IdMesa = dto.IdMesa,
                FechaHora = dto.FechaHora,
                FechaUltimaModificacion = DateTime.Now,
                HistorialEstatus = dto.HistorialEstatus.Select(e => new ReservaEstatus
                {
                    IdEstatus = e.IdEstatus,
                    Comentario = e.Comentario,
                    FechaUltimaModificacion = DateTime.Now
                }).ToList()
            };

            // 1️⃣ Guardar primero para obtener el Id
            var id = await _repo.AddReservaAsync(reserva);
            reserva.IdReserva = id;

            // 2️⃣ Crear la URL pública de la reserva
            var qrUrl = $"https://reservasnc.somee.com/api/Reserva/detalle/{id}";

            // 3️⃣ Generar el QR escaneable
            reserva.CodigoQR = GenerarCodigoQRBase64(qrUrl);

            // 4️⃣ Actualizar la reserva para guardar el QR
            await _repo.UpdateReservaAsync(reserva);

            // 5️⃣ Notificar en tiempo real (SignalR)
            await _hubContext.Clients.All.SendAsync("ReservaActualizada", $"Nueva reserva creada con ID: {id}");

            // 6️⃣ Enviar notificación push
            await _notificationService.SendPushToUserAsync(
                reserva.IdUsuario,
                "Reserva creada",
                "Tu reserva ha sido registrada correctamente."
            );

            return reserva;
        }

        private string GenerarCodigoQRBase64(string contenido)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(qrData))
            using (var bitmap = qrCode.GetGraphic(20))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                var base64 = Convert.ToBase64String(ms.ToArray());
                return $"data:image/png;base64,{base64}";
            }
        }


        /// <summary>
        /// Cambia el estado de una reserva agregando un nuevo registro en su historial.
        /// </summary>
        public async Task CambiarEstatusReservaAsync(int idReserva, int idEstatus, string? comentario = null)
        {
            await _repo.AddReservaEstatusAsync(idReserva, idEstatus, comentario);

            // Notificar en tiempo real
            await _hubContext.Clients.All.SendAsync("ReservaActualizada",
                $"Reserva {idReserva} cambió a estatus {idEstatus}.");

            // (Opcional) enviar push de actualización
            await _notificationService.SendPushToUserAsync(
                0, // puedes obtener el IdUsuario si lo necesitas con GetReservaByIdAsync
                "Estatus actualizado",
                "El estado de tu reserva ha sido actualizado."
            );
        }


        #endregion

        #region == Actualización (PUT) ==

        /// <summary>
        /// Actualiza la información de una reserva existente.
        /// </summary>
        public async Task UpdateReservaAsync(Reserva reserva)
        {
            // Recalcular QR (opcional, si la URL o datos cambian)
            var qrUrl = $"https://reservasnc.somee.com/api/Reserva/detalle/{reserva.IdReserva}";
            reserva.CodigoQR = GenerarCodigoQRBase64(qrUrl);

            await _repo.UpdateReservaAsync(reserva);

            await _hubContext.Clients.All.SendAsync("ReservaActualizada",
                $"Reserva {reserva.IdReserva} actualizada.");

            await _notificationService.SendPushToUserAsync(
                reserva.IdUsuario,
                "Reserva actualizada",
                "Tu reserva ha sido modificada correctamente."
            );
        }


        #endregion

        #region == Eliminación (DELETE) ==

        /// <summary>
        /// Elimina una reserva existente y notifica al usuario.
        /// </summary>
        public async Task DeleteReservaAsync(int id)
        {
            var reserva = await _repo.GetReservaByIdAsync(id);

            if (reserva != null)
            {
                await _notificationService.SendPushToUserAsync(
                    reserva.IdUsuario,
                    "Reserva eliminada",
                    "Tu reserva ha sido cancelada."
                );
            }

            await _repo.DeleteReservaAsync(id);

            await _hubContext.Clients.All.SendAsync("ReservaActualizada",
                $"Reserva {id} eliminada.");
        }

        #endregion
    }
}
