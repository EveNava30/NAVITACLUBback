using Microsoft.EntityFrameworkCore;
using ReservasNC.Application.DTOs;
using ReservasNC.Domain.Entities;

namespace ReservasNC.Infrastructure.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 🔹 Tablas principales
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Restaurante> Restaurantes { get; set; } = null!;
        public DbSet<Mesa> Mesas { get; set; } = null!;               //  Agregada correctamente
        public DbSet<Reserva> Reservas { get; set; } = null!;
        public DbSet<Estatus> Estatuses { get; set; } = null!;
        public DbSet<ReservaEstatus> ReservaEstatuses { get; set; } = null!;
        public DbSet<Reporte> Reportes { get; set; } = null!;
        public DbSet<FcmToken> FcmTokens { get; set; } = null!;

        // 🔹 DTOs o modelos sin clave (vistas o resultados de SP)
        public DbSet<MesaDto> MesasDisponibles { get; set; } = null!;
        public DbSet<ReservaDetalleDto> ReservasDetalleDto { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔸 Configuración de claves primarias
            modelBuilder.Entity<User>().HasKey(u => u.IdUser);
            modelBuilder.Entity<Role>().HasKey(r => r.IdRole);
            modelBuilder.Entity<Estatus>().HasKey(e => e.IdEstatus);
            modelBuilder.Entity<Restaurante>().HasKey(r => r.IdRestaurante);
            modelBuilder.Entity<Mesa>().HasKey(m => m.IdMesa);                // ✅ Clave agregada
            modelBuilder.Entity<Reserva>().HasKey(r => r.IdReserva);
            modelBuilder.Entity<ReservaEstatus>().HasKey(re => re.IdReservaEstatus);
            modelBuilder.Entity<Reporte>().HasKey(rep => rep.IdReporte);

            // 🔸 Configuración de índices únicos
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // 🔸 Entidades sin clave (DTOs / Vistas / SP)
            modelBuilder.Entity<MesaDto>()
                .HasNoKey()
                .ToView(null); // No está mapeada a una vista real

            modelBuilder.Entity<ReservaDetalleDto>()
                .HasNoKey()
                .ToView(null);

            // 🔸 Relaciones

            // User → Role
            modelBuilder.Entity<User>()
                .HasOne<Role>()
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.IdRole);

            // Reserva → Usuario
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.IdUsuario);

            // Reserva → Mesa
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Mesa)
                .WithMany(m => m.Reservas)
                .HasForeignKey(r => r.IdMesa);

            // ReservaEstatus → Reserva
            modelBuilder.Entity<ReservaEstatus>()
                .HasOne(re => re.Reserva)
                .WithMany(r => r.HistorialEstatus)
                .HasForeignKey(re => re.IdReserva);

            // ReservaEstatus → Estatus
            modelBuilder.Entity<ReservaEstatus>()
                .HasOne(re => re.Estatus)
                .WithMany()
                .HasForeignKey(re => re.IdEstatus);
        }
    }
}
