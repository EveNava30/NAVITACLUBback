namespace ReservasNC.Domain.Entities
{
    public class FcmToken
    {
        public int Id { get; set; }          
        public int IdUsuario { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
