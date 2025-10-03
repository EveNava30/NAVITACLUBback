using ReservasNC.Domain.Entities;

namespace ReservasNC.Domain.Entities;
public class User
{
    public int IdUser { get; set; }
    public required string Nombre { get; set; }
    public required string Email { get; set; }
    public required string Contrasena { get; set; }

    public string? Telefono { get; set; }
    public int IdRole { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime FechaUltimaModificacion { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
