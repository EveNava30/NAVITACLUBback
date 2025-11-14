namespace ReservasNC.Domain.Entities;
public class User
{
    public int IdUser { get; set; }           
    public string Nombre { get; set; }           
    public string Email { get; set; }         
    public string Contrasena { get; set; }       
    public string Telefono { get; set; }         
    public int IdRole { get; set; }              
    public DateTime FechaUltimaModificacion { get; set; }  

    // Campos nuevos para recuperación de contraseña
    public string? ResetToken { get; set; }
    public DateTime? TokenExpira { get; set; }
    public string? NombreRol { get; set; }

}
