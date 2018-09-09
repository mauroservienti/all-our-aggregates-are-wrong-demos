using Microsoft.EntityFrameworkCore;
using Shipping.Data.Models;
using System.Collections.Generic;

namespace Shipping.Data
{
    public class ShippingContext : DbContext
    {
        public static ShippingContext Create()
        {
            var db = new ShippingContext();
            db.Database.EnsureCreated();

            return db;
        }

        private ShippingContext() { }

        public DbSet<ProductShippingOptions> ProductShippingOptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\all-our-aggregates-are-wrong;Initial Catalog=Shipping;Integrated Security=True");
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
                        Option = "Express Delivery"
                    },
                    new ShippingOption()
                    {
                        Id = 2,
                        ProductShippingOptionsId = 1,
                        Option = "Regular mail"
                    },
                    new ShippingOption()
                    {
                        Id = 3,
                        ProductShippingOptionsId = 2,
                        Option = "Fantasy Delivery"
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
                    },
                    new ProductShippingOptions()
                    {
                        Id = 2,
                    }
                };
            }

        }
    }
}