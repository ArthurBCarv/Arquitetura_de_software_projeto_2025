using Microsoft.EntityFrameworkCore;
using Jogos.Models;

namespace Jogos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Jogo> Jogos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Jogo>().ToTable("Jogos");
            modelBuilder.Entity<Jogo>()
                .Property(j => j.Preco)
                .HasPrecision(10, 2);
        }
    }
}
