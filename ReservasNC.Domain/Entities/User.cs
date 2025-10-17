using ReservasNC.Domain.Entities;

namespace ReservasNC.Domain.Entities;
public class User
{
    public int IdUser { get; set; }               // PK
    public string Nombre { get; set; }           // Nombre completo
    public string Email { get; set; }            // Email único
    public string Contrasena { get; set; }       // Contraseña
    public string Telefono { get; set; }         // Teléfono (opcional)
    public int IdRole { get; set; }              // FK a Rol
    public DateTime FechaUltimaModificacion { get; set; }  // Control de actualización
}
//comentario 3 Juan 
