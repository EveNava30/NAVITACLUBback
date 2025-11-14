using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Infrastructure.DataContexts;

namespace ReservasNC.Infrastructure.Persistence
{
    public class FcmTokenRepository : IFcmTokenRepository
    {
        private readonly AppDbContext _context;

        public FcmTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveTokenAsync(FcmToken token)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SaveFcmToken";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@IdUsuario", token.IdUsuario));
            command.Parameters.Add(new SqlParameter("@Token", token.Token));

            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<string?> GetTokenByUserAsync(int idUsuario)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "GetFcmTokenByUser";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));

            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();

            return result?.ToString();
        }
    }
}
