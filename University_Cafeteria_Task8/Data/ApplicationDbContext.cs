
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using University_Cafeteria_Task8.Models;

namespace University_Cafeteria_Task8.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<SupplyRequest> SupplyRequests { get; set; }

        public DbSet<RequestItem> RequestItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)");
            });

            builder.Entity<SupplyRequest>(entity =>
            {
                entity.ToTable("SupplyRequests");

                entity.Property(x => x.TotalPrice)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<RequestItem>(entity =>
            {
                entity.ToTable("RequestItems");

                entity.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.SupplyRequest)
                    .WithMany(x => x.RequestItems)
                    .HasForeignKey(x => x.SupplyRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
