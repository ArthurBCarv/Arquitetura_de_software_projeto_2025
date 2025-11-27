using Microsoft.EntityFrameworkCore;
using Compras.Models;

namespace Compras.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compra>().ToTable("Compras");
            modelBuilder.Entity<Compra>()
                .Property(c => c.PrecoCompra)
                .HasPrecision(10, 2);
            
            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.UsuarioId);
            
            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.JogoId);
        }
    }
}
