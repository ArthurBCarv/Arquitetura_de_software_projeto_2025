using Microsoft.EntityFrameworkCore;
using Jogos.Models;

namespace Jogos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Jogo> Jogos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jogo>()
                .Property(j => j.Preco)
                .HasColumnType("decimal(18,2)");
        }
    }
}
