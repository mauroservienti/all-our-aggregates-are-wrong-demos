using Microsoft.EntityFrameworkCore;
using Sales.Data.Models;

namespace Sales.Data
{
    public class SalesContext : DbContext
    {
        public static SalesContext Create()
        {
            var db = new SalesContext();
            db.Database.EnsureCreated();

            return db;
        }

        private SalesContext() { }

        public DbSet<ProductPrice> ProductsPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\all-our-aggregates-are-wrong;Initial Catalog=Sales;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductPrice>().HasData(Initial.Data());

            //var cartEntity = modelBuilder.Entity<ShoppingCart>();

            //cartEntity
            //    .HasKey(c => c.Id)
            //    .HasMany(c => c.Items)
            //    .WithRequired()
            //    .HasForeignKey(k => k.CartId)
            //    .WillCascadeOnDelete();

            //cartEntity
            //    .Property(c => c.Id)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            base.OnModelCreating(modelBuilder);
        }

        internal static class Initial
        {
            internal static ProductPrice[] Data()
            {
                return new[]
                {
                    new ProductPrice()
                    {
                        Id = 1,
                        Price = 10.00m
                    },
                    new ProductPrice()
                    {
                        Id = 2,
                        Price = 100.00m,
                    }
                };
            }

        }
    }
}
