using Microsoft.EntityFrameworkCore;
using Shipping.Data.Models;

namespace Shipping.Data
{
    public class ShippingContext : DbContext
    {
        public ShippingContext() { }

        public DbSet<ProductShippingOptions> ProductShippingOptions { get; set; }

        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=localhost;Port=8432;Username=db_user;Password=P@ssw0rd;Database=shipping_database");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var productShippingOptionsEntity = modelBuilder.Entity<ProductShippingOptions>();
            var shippingOptionsEntity = modelBuilder.Entity<ShippingOption>();

            shippingOptionsEntity
                .HasOne<ProductShippingOptions>()
                .WithMany(pso => pso.Options)
                .IsRequired()
                .HasForeignKey(so => so.ProductShippingOptionsId)
                .OnDelete(DeleteBehavior.Cascade);

            shippingOptionsEntity.HasData(Initial.ShippingOptions());
            productShippingOptionsEntity.HasData(Initial.ProductShippingOptions());

            var shoppingCartItemEntity = modelBuilder.Entity<ShoppingCartItem>();
            shoppingCartItemEntity.Property(i => i.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

        internal static class Initial
        {
            internal static ShippingOption[] ShippingOptions()
            {
                return new[]
                {
                    new ShippingOption()
                    {
                        Id = 1,
                        ProductShippingOptionsId = 1,
                        Option = "Express Delivery",
                        EstimatedMinDeliveryDays = 1,
                        EstimatedMaxDeliveryDays = 3,
                    },
                    new ShippingOption()
                    {
                        Id = 2,
                        ProductShippingOptionsId = 1,
                        Option = "Regular mail",
                        EstimatedMinDeliveryDays = 4,
                        EstimatedMaxDeliveryDays = 12,
                    },
                    new ShippingOption()
                    {
                        Id = 3,
                        ProductShippingOptionsId = 2,
                        Option = "Fantasy Delivery",
                        EstimatedMinDeliveryDays = int.MaxValue,
                        EstimatedMaxDeliveryDays = int.MaxValue,
                    },
                };
            }

            internal static ProductShippingOptions[] ProductShippingOptions()
            {
                return new[]
                {
                    new ProductShippingOptions()
                    {
                        Id = 1,
                        ProductId = 1,
                    },
                    new ProductShippingOptions()
                    {
                        Id = 2,
                        ProductId = 2,
                    }
                };
            }

        }
    }
}