namespace ReservasNC.Domain.Entities
{
    public class ReservaDetalleData
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = "";
        public int IdMesa { get; set; }
        public string NumeroMesa { get; set; } = "";
        public string NombreRestaurante { get; set; } = "";
        public string CodigoQR { get; set; } = "";
        public DateTime FechaHora { get; set; }
        public List<HistorialEstatusData> Historial { get; set; } = new List<HistorialEstatusData>();
    }
}
