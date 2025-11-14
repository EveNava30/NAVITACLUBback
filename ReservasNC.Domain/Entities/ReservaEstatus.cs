namespace ReservasNC.Domain.Entities
{
    public class ReservaEstatus
    {
        public int IdReservaEstatus { get; set; }
        public int IdReserva { get; set; }
        public Reserva? Reserva { get; set; }
        public int IdEstatus { get; set; }
        public Estatus Estatus { get; set; } = null!;
        public string? Comentario { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
