namespace ReservasNC.Infrastructure.DataContexts;

using Microsoft.EntityFrameworkCore;
using ReservasNC.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }  // DbSet para User

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.IdUser);
    }
}
