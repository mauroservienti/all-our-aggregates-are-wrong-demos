using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Data
{
    public class MarketingContext : DbContext
    {
        public static MarketingContext Create()
        {
            var db = new MarketingContext();
            db.Database.EnsureCreated();

            return db;
        }

        private MarketingContext() { }

        public DbSet<ProductDetails> ProductsDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\all-our-aggregates-are-wrong;Initial Catalog=Marketing;Integrated Security=True");
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
