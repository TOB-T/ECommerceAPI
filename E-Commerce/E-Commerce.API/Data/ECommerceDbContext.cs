using E_Commerce.API.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace E_Commerce.API.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set the precision and scale for the Price property in Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
               .HasPrecision(18, 2); // 18 digits total, 2 after the decimal point

            // Set the precision and scale for the Price property in OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(18, 2); // 18 digits total, 2 after the decimal point

            // Configure the relationship between Order and IdentityUser
            //modelBuilder.Entity<Order>()
            //.HasOne(o => o.User)
            //.WithMany()
            //.HasForeignKey(o => o.UserId)
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Restrict);

            // Ignore IdentityUser entity
            modelBuilder.Ignore<IdentityUser>();

            base.OnModelCreating(modelBuilder);
        }

       


    }
}
