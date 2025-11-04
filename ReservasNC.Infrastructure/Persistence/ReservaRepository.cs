using Microsoft.EntityFrameworkCore;
using ReservasNC.Application.DTOs;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Infrastructure.DataContexts;

namespace ReservasNC.Infrastructure.Persistence
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> GetReservasAsync()
        {
            return await _context.Set<Reserva>()
                .FromSqlRaw("EXEC GetReservas")
                .ToListAsync();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            var data = await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservaById {id}")
                .ToListAsync();
            return data.FirstOrDefault();
        }
        

        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorFecha {fecha}")
                .ToListAsync();
        }

        public async Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorRangoFechas {fechaInicio}, {fechaFin}")
                .ToListAsync();
        }
        public async Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorUsuario {idUsuario}")
                .ToListAsync();
        }

        public async Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorRestaurante {idRestaurante}")
                .ToListAsync();
        }
        public async Task<ReservaDetalleData?> GetReservaDetalleAsync(int id)
        {
            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "GetReservaDetalle";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var param = command.CreateParameter();
            param.ParameterName = "@IdReserva";
            param.Value = id;
            command.Parameters.Add(param);

            ReservaDetalleData? detalle = null;

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (detalle == null)
                {
                    detalle = new ReservaDetalleData
                    {
                        IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? "" : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                        IdMesa = reader.GetInt32(reader.GetOrdinal("IdMesa")),
                        NumeroMesa = reader.IsDBNull(reader.GetOrdinal("NumeroMesa"))
                  ? ""
                  : reader.GetInt32(reader.GetOrdinal("NumeroMesa")).ToString(), 
                        NombreRestaurante = reader.IsDBNull(reader.GetOrdinal("NombreRestaurante")) ? "" : reader.GetString(reader.GetOrdinal("NombreRestaurante")),
                        CodigoQR = reader.IsDBNull(reader.GetOrdinal("CodigoQR")) ? "" : reader.GetString(reader.GetOrdinal("CodigoQR")),
                        FechaHora = reader.GetDateTime(reader.GetOrdinal("FechaHora")),
                        Historial = new List<HistorialEstatusData>()
                    };

                }

                if (!reader.IsDBNull(reader.GetOrdinal("IdReservaEstatus")))
                {
                    detalle.Historial.Add(new HistorialEstatusData
                    {
                        IdReservaEstatus = reader.GetInt32(reader.GetOrdinal("IdReservaEstatus")),
                        IdEstatus = reader.GetInt32(reader.GetOrdinal("IdEstatus")),
                        NombreEstatus = reader.GetString(reader.GetOrdinal("NombreEstatus")),
                        Comentario = reader.GetString(reader.GetOrdinal("Comentario")),
                        FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion"))
                    });
                }
            }

            await connection.CloseAsync();
            return detalle;
        }







        /*public async Task<int> AddReservaAsync(Reserva reserva)
        {
            var result = await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC AddReserva {reserva.IdUsuario}, {reserva.IdMesa}, {reserva.FechaHora}")
                .ToListAsync();
            return result.FirstOrDefault()?.IdReserva ?? 0;
        }*/
        /* public async Task<int> AddReservaAsync(Reserva reserva)
         {
             using var command = _context.Database.GetDbConnection().CreateCommand();
             command.CommandText = "CreateReserva";
             command.CommandType = System.Data.CommandType.StoredProcedure;

             command.Parameters.Add(new SqlParameter("@IdUsuario", reserva.IdUsuario));
             command.Parameters.Add(new SqlParameter("@IdMesa", reserva.IdMesa));
             command.Parameters.Add(new SqlParameter("@CodigoQR", (object?)reserva.CodigoQR ?? DBNull.Value));
             command.Parameters.Add(new SqlParameter("@FechaHora", reserva.FechaHora));
             command.Parameters.Add(new SqlParameter("@IdEstatusInicial", reserva.HistorialEstatus.First().IdEstatus));
             command.Parameters.Add(new SqlParameter("@Comentario", (object?)reserva.HistorialEstatus.First().Comentario ?? DBNull.Value));

             await _context.Database.OpenConnectionAsync();
             var result = await command.ExecuteScalarAsync();
             await _context.Database.CloseConnectionAsync();

             return Convert.ToInt32(result);
         }*/
        public async Task<int> AddReservaAsync(Reserva reserva)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "CreateReserva"; // Tu SP que inserta reserva + estatus
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@IdUsuario", reserva.IdUsuario));
            command.Parameters.Add(new SqlParameter("@IdMesa", reserva.IdMesa));
            command.Parameters.Add(new SqlParameter("@CodigoQR", (object?)reserva.CodigoQR ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@FechaHora", reserva.FechaHora));
            command.Parameters.Add(new SqlParameter("@IdEstatusInicial", reserva.HistorialEstatus.First().IdEstatus));
            command.Parameters.Add(new SqlParameter("@Comentario", (object?)reserva.HistorialEstatus.First().Comentario ?? DBNull.Value));

            await _context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await _context.Database.CloseConnectionAsync();

            return Convert.ToInt32(result);
        }




        public async Task UpdateReservaAsync(Reserva reserva)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC UpdateReserva {reserva.IdReserva}, {reserva.IdUsuario}, {reserva.IdMesa}, {reserva.FechaHora}");
        }

        public async Task DeleteReservaAsync(int id)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC DeleteReserva {id}");
        }
    }
}
