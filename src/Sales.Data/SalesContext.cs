using Microsoft.EntityFrameworkCore;
using Sales.Data.Models;

namespace Sales.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext() { }

        public DbSet<ProductPrice> ProductsPrices { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database=sales_database");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductPrice>().HasData(Initial.Data());

            var shoppingCartItemEntity = modelBuilder.Entity<ShoppingCartItem>();
            var shoppingCartEntity = modelBuilder.Entity<ShoppingCart>();

            shoppingCartItemEntity
                .HasOne<ShoppingCart>()
                .WithMany(sc => sc.Items)
                .IsRequired()
                .HasForeignKey(so => so.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        static class Initial
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
