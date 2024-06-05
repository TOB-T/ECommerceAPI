using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Migrate and seed the Identity database
                var identityContext = services.GetRequiredService<ECommerceIdentityDbContext>();
                await identityContext.Database.MigrateAsync();
                await SeedIdentityData(services);

                // Migrate the Application database
                var appContext = services.GetRequiredService<ECommerceDbContext>();
                await appContext.Database.MigrateAsync();
            }
        }

        private static async Task SeedIdentityData(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Reader", "Writer" };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var testUser = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                NormalizedUserName = "TESTUSER",
                NormalizedEmail = "TESTUSER@EXAMPLE.COM",
                EmailConfirmed = true
            };

            if (await userManager.FindByNameAsync(testUser.UserName) == null)
            {
                await userManager.CreateAsync(testUser, "Test@123");
                await userManager.AddToRoleAsync(testUser, "Reader");
            }
        }
    }
}
