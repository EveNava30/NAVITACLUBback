namespace ReservasNC.Infrastructure.Persistence
{
    /// <summary>
    /// Repositorio responsable de las operaciones de persistencia relacionadas con los usuarios.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de usuarios.
        /// </summary>
        /// <param name="context">Contexto de base de datos de la aplicación.</param>
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        #region  CRUD BÁSICO

        /// <summary>
        /// Agrega un nuevo usuario utilizando un procedimiento almacenado.
        /// </summary>
        /// <param name="user">Entidad de usuario a agregar.</param>
        /// <returns>Id del usuario creado.</returns>
        public async Task<int> AddUserAsync(User user)
        {
            var result = await _context.Users
                .FromSqlInterpolated($"EXEC AddUser {user.Nombre}, {user.Email}, {user.Contrasena}, {user.Telefono}, {user.IdRole}")
                .ToListAsync();

            return result.FirstOrDefault()?.IdUser ?? 0;
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users
                .FromSqlRaw("EXEC GetUsers")
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="idUser">Id del usuario.</param>
        /// <returns>Entidad de usuario si existe; de lo contrario, null.</returns>
        public async Task<User?> GetByIdAsync(int idUser)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC GetUserById {idUser}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="user">Entidad de usuario con datos actualizados.</param>
        public async Task UpdateUserAsync(User user)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC UpdateUser {user.IdUser}, {user.Nombre}, {user.Email}, {user.Telefono}, {user.IdRole}");
        }

        /// <summary>
        /// Elimina un usuario por su Id.
        /// </summary>
        /// <param name="idUser">Id del usuario a eliminar.</param>
        public async Task DeleteUserAsync(int idUser)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC DeleteUser {idUser}");
        }

        #endregion

        #region  LOGIN

        /// <summary>
        /// Inicia sesión del usuario según su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <returns>Usuario encontrado o null si no existe.</returns>
        public async Task<User?> LoginUserAsync(string email)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"EXEC LoginUser {email}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        #endregion

        #region CAMBIO DE CONTRASEÑA

        /// <summary>
        /// Cambia la contraseña del usuario especificado.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <param name="nuevaContrasena">Nueva contraseña (ya en texto plano o cifrada según la lógica de negocio).</param>
        public async Task ChangePasswordAsync(string email, string nuevaContrasena)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC ChangePassword {email}, {nuevaContrasena}");
        }

        #endregion

        #region  RECUPERACIÓN DE CONTRASEÑA

        /// <summary>
        /// Guarda un token temporal para restablecer la contraseña.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <param name="token">Token de restablecimiento.</param>
        /// <param name="expiracion">Fecha de expiración del token.</param>
        public async Task SaveResetTokenAsync(string email, string token, DateTime expiracion)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Usuario SET ResetToken = {token}, TokenExpira = {expiracion} WHERE Email = {email}");
        }

        /// <summary>
        /// Obtiene un usuario a partir de su token de restablecimiento.
        /// </summary>
        /// <param name="token">Token de restablecimiento.</param>
        /// <returns>Usuario asociado al token, si existe.</returns>
        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"SELECT * FROM Usuario WHERE ResetToken = {token}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <returns>Usuario correspondiente, si existe.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var data = await _context.Users
                .FromSqlInterpolated($"SELECT * FROM Usuario WHERE Email = {email}")
                .ToListAsync();

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Restablece la contraseña del usuario mediante un token válido.
        /// </summary>
        /// <param name="token">Token de restablecimiento.</param>
        /// <param name="hashedPassword">Nueva contraseña ya hasheada.</param>
        /// <returns>True si el restablecimiento fue exitoso; false si el token no es válido.</returns>
        public async Task<bool> ResetPasswordAsync(string token, string hashedPassword)
        {
            var user = await GetUserByResetTokenAsync(token);
            if (user == null) return false;

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Usuario 
                SET Contrasena = {hashedPassword}, 
                    ResetToken = NULL, 
                    TokenExpira = NULL,
                    FechaUltimaModificacion = GETDATE()
                WHERE IdUser = {user.IdUser}");

            return true;
        }

        #endregion
    }
}
