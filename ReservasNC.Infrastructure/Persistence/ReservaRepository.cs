namespace ReservasNC.Infrastructure.Persistence
{
    /// <summary>
    /// Repositorio encargado de manejar las operaciones de acceso a datos relacionadas con las reservas.
    /// Implementa la interfaz <see cref="IReservaRepository"/>.
    /// </summary>
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de EF Core.</param>
        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        #region 🔍 CONSULTAS GENERALES
        // ============================================================

        /// <summary>
        /// Obtiene todas las reservas existentes.
        /// </summary>
        public async Task<List<Reserva>> GetReservasAsync()
        {
            return await _context.Set<Reserva>()
                .FromSqlRaw("EXEC GetReservas")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una reserva por su identificador único.
        /// </summary>
        /// <param name="id">ID de la reserva.</param>
        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            var data = await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservaById {id}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el detalle completo de una reserva, incluyendo su historial de estatus.
        /// </summary>
        /// <param name="id">ID de la reserva.</param>
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

        #endregion

        // ============================================================
        #region 📅 CONSULTAS POR FECHA / USUARIO / RESTAURANTE
        // ============================================================

        /// <summary>
        /// Obtiene todas las reservas registradas en una fecha específica.
        /// </summary>
        /// <param name="fecha">Fecha de búsqueda.</param>
        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime fecha)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorFecha {fecha}")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las reservas registradas dentro de un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del rango.</param>
        /// <param name="fechaFin">Fecha final del rango.</param>
        public async Task<List<Reserva>> GetReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorRangoFechas {fechaInicio}, {fechaFin}")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las reservas asociadas a un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        public async Task<List<Reserva>> GetReservasPorUsuarioAsync(int idUsuario)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorUsuario {idUsuario}")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las reservas asociadas a un restaurante específico.
        /// </summary>
        /// <param name="idRestaurante">ID del restaurante.</param>
        public async Task<List<Reserva>> GetReservasPorRestauranteAsync(int idRestaurante)
        {
            return await _context.Set<Reserva>()
                .FromSqlInterpolated($"EXEC GetReservasPorRestaurante {idRestaurante}")
                .ToListAsync();
        }

        #endregion

        // ============================================================
        #region 🧩 OPERACIONES CRUD
        // ============================================================

        /// <summary>
        /// Inserta una nueva reserva en la base de datos utilizando un procedimiento almacenado.
        /// </summary>
        /// <param name="reserva">Entidad de la reserva a agregar.</param>
        public async Task<int> AddReservaAsync(Reserva reserva)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "CreateReserva";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@IdUsuario", reserva.IdUsuario));
            command.Parameters.Add(new SqlParameter("@IdMesa", reserva.IdMesa));
            command.Parameters.Add(new SqlParameter("@CodigoQR", (object?)reserva.CodigoQR ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@FechaHora", reserva.FechaHora));
            //command.Parameters.Add(new SqlParameter("@HoraInicio", reserva.HoraInicio));
            //command.Parameters.Add(new SqlParameter("@Duracion", reserva.Duracion));
            command.Parameters.Add(new SqlParameter("@IdEstatusInicial", reserva.HistorialEstatus.First().IdEstatus));
            command.Parameters.Add(new SqlParameter("@Comentario", (object?)reserva.HistorialEstatus.First().Comentario ?? DBNull.Value));

            await _context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await _context.Database.CloseConnectionAsync();

            return Convert.ToInt32(result);
        }
        /// <summary>
        /// Inserta nuevo estado de la reserva
        /// </summary>
        public async Task AddReservaEstatusAsync(int idReserva, int idEstatus, string? comentario = null)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC AddReservaEstatus {idReserva}, {idEstatus}, {comentario}");
        }

        /// <summary>
        /// Actualiza los datos de una reserva existente.
        /// </summary>
        /// <param name="reserva">Entidad de reserva con los datos actualizados.</param>
       public async Task UpdateReservaAsync(Reserva reserva)
{
    await _context.Database.ExecuteSqlInterpolatedAsync($@"
        EXEC UpdateReserva 
            {reserva.IdReserva}, 
            {reserva.IdUsuario}, 
            {reserva.IdMesa}, 
            {reserva.FechaHora}, 
            {reserva.CodigoQR}");
}


        /// <summary>
        /// Elimina una reserva de la base de datos por su identificador.
        /// </summary>
        /// <param name="id">ID de la reserva a eliminar.</param>
        public async Task DeleteReservaAsync(int id)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC DeleteReserva {id}");
        }

        #endregion
    }
}
