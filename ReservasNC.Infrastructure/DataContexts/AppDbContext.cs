namespace ReservasNC.Infrastructure.DataContexts;

using Microsoft.EntityFrameworkCore;
using ReservasNC.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Estatus> Estatuses => Set<Estatus>();
    public DbSet<ReservaEstatus> ReservaEstatuses => Set<ReservaEstatus>();
    public DbSet<Reporte> Reportes => Set<Reporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // map names exactly
        modelBuilder.Entity<Role>().ToTable("Rol");
        modelBuilder.Entity<User>().ToTable("Usuario");
        modelBuilder.Entity<Restaurante>().ToTable("Restaurante");
        modelBuilder.Entity<Mesa>().ToTable("Mesa");
        modelBuilder.Entity<Reserva>().ToTable("Reserva");
        modelBuilder.Entity<Estatus>().ToTable("Estatus");
        modelBuilder.Entity<ReservaEstatus>().ToTable("ReservaEstatus");
        modelBuilder.Entity<Reporte>().ToTable("Reporte");

        // keys and relationships (EF infers but explicit is good)
        // Users
        modelBuilder.Entity<User>().HasKey(u => u.IdUser);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.IdRole);

        // Roles
        modelBuilder.Entity<Role>().HasKey(r => r.IdRole);

        // Estatus
        modelBuilder.Entity<Estatus>().HasKey(e => e.IdEstatus);

        // Restaurantes
        modelBuilder.Entity<Restaurante>().HasKey(r => r.IdRestaurante);

        // Mesas
        modelBuilder.Entity<Mesa>().HasKey(m => m.IdMesa);
        modelBuilder.Entity<Mesa>()
            .HasOne(m => m.Restaurante).WithMany(r => r.Mesas).HasForeignKey(m => m.IdRestaurante);

        // Reservas
        modelBuilder.Entity<Reserva>().HasKey(r => r.IdReserva);
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Usuario).WithMany(u => u.Reservas).HasForeignKey(r => r.IdUsuario);
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Mesa).WithMany(m => m.Reservas).HasForeignKey(r => r.IdMesa);

        // ReservaEstatus
        modelBuilder.Entity<ReservaEstatus>().HasKey(re => re.IdReservaEstatus);
        modelBuilder.Entity<ReservaEstatus>()
            .HasOne(re => re.Reserva).WithMany(r => r.HistorialEstatus).HasForeignKey(re => re.IdReserva);
        modelBuilder.Entity<ReservaEstatus>()
            .HasOne(re => re.Estatus).WithMany(e => e.ReservaEstatus).HasForeignKey(re => re.IdEstatus);

        // Reportes
        modelBuilder.Entity<Reporte>().HasKey(rep => rep.IdReporte);
        modelBuilder.Entity<Reporte>()
            .HasOne(rep => rep.Usuario).WithMany(u => u.Reportes).HasForeignKey(rep => rep.IdUsuario);

        // seed data for Roles and Estatus
        modelBuilder.Entity<Role>().HasData(
     new Role { IdRole = 1, NombreRol = "Cliente", FechaUltimaModificacion = new DateTime(2025, 10, 3) },
     new Role { IdRole = 2, NombreRol = "Mesero", FechaUltimaModificacion = new DateTime(2025, 10, 3) },
     new Role { IdRole = 3, NombreRol = "Administrador", FechaUltimaModificacion = new DateTime(2025, 10, 3) }
 );

        modelBuilder.Entity<Estatus>().HasData(
            new Estatus { IdEstatus = 1, Nombre = "Reservado", FechaUltimaModificacion = new DateTime(2025, 10, 3) },
            new Estatus { IdEstatus = 2, Nombre = "Completado", FechaUltimaModificacion = new DateTime(2025, 10, 3) },
            new Estatus { IdEstatus = 3, Nombre = "Cancelado", FechaUltimaModificacion = new DateTime(2025, 10, 3) }
        );

    }
}
