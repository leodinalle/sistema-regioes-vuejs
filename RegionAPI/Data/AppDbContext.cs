using Microsoft.EntityFrameworkCore;
using RegionAPI.Model;

namespace RegionAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Region>()
                .HasIndex(r => new { r.Uf, r.Nome })
                .IsUnique(); // Tentativa de evitar duplicação com o msm nome e msm UF.
            base.OnModelCreating(modelBuilder);
        }
    }
}
