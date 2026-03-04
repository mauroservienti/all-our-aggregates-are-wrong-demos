using System;
using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Data
{
    public class MarketingContext : DbContext
    {
        public DbSet<ProductDetails> ProductsDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_MARKETING_CONNECTION_STRING")
                                   ?? "Host=localhost;Port=6432;Username=db_user;Password=P@ssw0rd;Database=marketing_database";
            
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDetails>()
                .HasData(Initial.Data());

            base.OnModelCreating(modelBuilder);
        }

        internal static class Initial
        {
            internal static ProductDetails[] Data()
            {
                return new[]
                {
                    new ProductDetails()
                    {
                        Id = 1,
                        Name = "Banana Holder",
                        Description = "Outdoor travel cute banana protector storage box"
                    },
                    new ProductDetails()
                    {
                        Id = 2,
                        Name = "Nokia Lumia 635",
                        Description = "Amazing phone, unfortunately not understood by market"
                    }
                };
            }

        }
    }
}
