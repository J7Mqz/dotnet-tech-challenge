using Microsoft.EntityFrameworkCore;

namespace QueueWorker.Data
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

        public DbSet<ProductEntity> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>()
                .HasIndex(p => p.ExternalId)
                .IsUnique();

            modelBuilder.Entity<ProductEntity>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}
