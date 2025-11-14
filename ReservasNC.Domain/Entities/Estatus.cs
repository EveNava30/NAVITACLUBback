namespace ReservasNC.Domain.Entities
{
    public class Estatus
    {
        public int IdEstatus { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaUltimaModificacion { get; set; }
        public ICollection<ReservaEstatus> ReservaEstatus { get; set; } = new List<ReservaEstatus>();
    }

}
