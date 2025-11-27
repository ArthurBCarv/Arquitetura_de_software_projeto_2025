using Microsoft.EntityFrameworkCore;
using Compras.Models;

namespace Compras.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Compra>()
                .Property(c => c.ValorPago)
                .HasColumnType("decimal(18,2)");
        }
    }
}
