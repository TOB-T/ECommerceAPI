using E_Commerce.API.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Data
{
    public class ECommerceIdentityDbContext : IdentityDbContext
    {
        public ECommerceIdentityDbContext(DbContextOptions<ECommerceIdentityDbContext> IdentityOptions) : base(IdentityOptions)
        {
            
        }

       

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            // Configure the relationship between Order and IdentityUser
            builder.Entity<Order>()
                .HasOne(o => o.User) // Each Order has one User
                .WithMany() // Each User can have many Orders
                .HasForeignKey(o => o.UserId) // The foreign key in the Order table is UserId
                .IsRequired() // UserId cannot be null
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of User if they have Orders


            var readerRoleId = "a33383cb-b9e5-488c-8a53-f94a302ad90d";
            var WriterRoleId = "7aa19040-bd5c-43dc-aea7-da74f36a526f";

            var roleModels = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },

                 new IdentityRole()
                {
                    Id = WriterRoleId,
                    ConcurrencyStamp = WriterRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }

            };

            builder.Entity<IdentityRole>().HasData(roleModels);

        }
    }

}
