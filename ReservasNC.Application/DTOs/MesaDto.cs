namespace ReservasNC.Application.DTOs
{
    public class MesaDto
    {
        public int IdMesa { get; set; }
        public int NumeroMesa { get; set; }
        public int Capacidad { get; set; }
        public int IdRestaurante { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
