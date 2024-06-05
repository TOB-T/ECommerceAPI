
using E_Commerce.API.Data;
using E_Commerce.API.Interfaces;
using E_Commerce.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ECommerceIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceIdentityConnectionString")));

            builder.Services.AddDbContext<ECommerceDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceConnectionString")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ECommerceIdentityDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt :Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
               });

            builder.Services.AddIdentityCore<IdentityUser>().AddRoles<IdentityRole>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("ECommerce")
            .AddEntityFrameworkStores<ECommerceIdentityDbContext>()
            .AddDefaultTokenProviders();

              builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

            });

            // Register AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register repositories and interfaces
            builder.Services.AddScoped<IOrderInterface, OrderRepository>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(); // Enable serving static files

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var identityContext = services.GetRequiredService<ECommerceIdentityDbContext>();
                    identityContext.Database.Migrate(); // Apply migrations to the Identity database
                    SeedData.Initialize(services).Wait(); // Seed initial data for the Identity database

                    var appContext = services.GetRequiredService<ECommerceDbContext>();
                    appContext.Database.Migrate(); // Apply migrations to the Application database
                }
                catch (Exception ex)
                {
                    // Log any errors here
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }

            app.Run();
        }
    }
}