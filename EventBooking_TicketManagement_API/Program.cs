using Applications.Interfaces;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Applications.Services;
using Domains.Entities;
using EventBooking_TicketManagement_API.Services;
using Infrastructure.Repository;
using Infrastructure.Security;
using Infrastructures.DbContexts;
using Infrastructures.Repositories;
using Infrastructures.Repository;
using Infrastructures.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventBooking_TicketManagement_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------------------------------------------------
            // 🗃️ Database
            // ---------------------------------------------------------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

            // ---------------------------------------------------------
            // 🧩 Repositories
            // ---------------------------------------------------------
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IVenueRepository, VenueRepository>();
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();


            // ---------------------------------------------------------
            // 🧠 Services
            // ---------------------------------------------------------
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ICityService, CityService>();
            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<IVenueService, VenueService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICountryService, CountryService>();

            // ---------------------------------------------------------
            // 🔐 Security Services
            // ---------------------------------------------------------
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ---------------------------------------------------------
            // 🌐 MVC + Swagger
            // ---------------------------------------------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ---------------------------------------------------------
            // 🌍 CORS (allow Blazor + credentials for cookies)
            // ---------------------------------------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient", policy =>
                {
                    policy.WithOrigins("https://localhost:7117") // Blazor client URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // ✅ Required for cookie auth
                });
            });

            // ---------------------------------------------------------
            // 🔑 JWT Authentication
            // ---------------------------------------------------------
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // ✅ Extract JWT token from cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("jwt", out var token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // ---------------------------------------------------------
            // 🧾 Build Application
            // ---------------------------------------------------------
            var app = builder.Build();

            // ---------------------------------------------------------
            // 🧪 Database Seeding (Admin)
            // ---------------------------------------------------------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                db.Database.Migrate();

                if (!db.Users.Any())
                {
                    var admin = new AdminUser
                    {
                        Username = "admin",
                        Email = "admin@bookingapp.com",
                        PasswordHash = passwordHasher.HashPassword("Admin@123"),
                        Role = "Admin",
                        PhoneNumber = "9999999999"
                    };

                    db.Users.Add(admin);
                    db.SaveChanges();
                }
            }
            // Seed data
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                await CountrySeeder.SeedAsync(db);
                await StateSeeder.SeedAsync(db);
            }

            // ---------------------------------------------------------
            // 🚀 Middleware Pipeline
            // ---------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowBlazorClient");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                Console.WriteLine("✅ Connected DB: " + db.Database.GetDbConnection().Database);
                Console.WriteLine("✅ Connected Server: " + db.Database.GetDbConnection().DataSource);
                Console.WriteLine("✅ Venue Count: " + db.Venues.Count());
            }


            app.Run();
        }
    }
}
