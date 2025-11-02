namespace ReservasNC.Infrastructure.DataContexts;

using Microsoft.EntityFrameworkCore;
using ReservasNC.Application.DTOs;
using ReservasNC.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Restaurante> Restaurantes { get; set; }
    public DbSet<Mesa> Mesas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Estatus> Estatuses { get; set; }
    public DbSet<ReservaEstatus> ReservaEstatuses { get; set; }
    public DbSet<Reporte> Reportes { get; set; }
    public DbSet<ReservaDetalleDto> ReservasDetalleDto { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasKey(u => u.IdUser);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Role>().HasKey(r => r.IdRole);
        modelBuilder.Entity<Estatus>().HasKey(e => e.IdEstatus);
        modelBuilder.Entity<Restaurante>().HasKey(r => r.IdRestaurante);
        modelBuilder.Entity<Mesa>().HasKey(m => m.IdMesa);
        modelBuilder.Entity<Reserva>().HasKey(r => r.IdReserva);
        modelBuilder.Entity<ReservaEstatus>().HasKey(re => re.IdReservaEstatus);
        modelBuilder.Entity<Reporte>().HasKey(rep => rep.IdReporte);
        modelBuilder.Entity<ReservaDetalleDto>()
            .HasNoKey()
            .ToView(null); 


        // Relación User → Role
        modelBuilder.Entity<User>()
            .HasOne<Role>()
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.IdRole);

        // Relación Reserva → Usuario
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.IdUsuario);

        // Relación Reserva → Mesa
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Mesa)
            .WithMany(m => m.Reservas)
            .HasForeignKey(r => r.IdMesa);

        // Relación ReservaEstatus → Reserva
        modelBuilder.Entity<ReservaEstatus>()
            .HasOne(re => re.Reserva)
            .WithMany(r => r.HistorialEstatus)
            .HasForeignKey(re => re.IdReserva);

        // Relación ReservaEstatus → Estatus
        modelBuilder.Entity<ReservaEstatus>()
            .HasOne(re => re.Estatus)
            .WithMany()
            .HasForeignKey(re => re.IdEstatus);

    }
}
