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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EventBooking_TicketManagement_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------------------------------------------------
            // Database
            // ---------------------------------------------------------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

            // ---------------------------------------------------------
            // Repositories
            // ---------------------------------------------------------
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IVenueRepository, VenueRepository>();
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
            builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();

            // ---------------------------------------------------------
            // Services
            // ---------------------------------------------------------
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ICityService, CityService>();
            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<IVenueService, VenueService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IManagerService, ManagerServices>();
            builder.Services.AddScoped<IEventCategoryService,EventCategoryService>();

            // ---------------------------------------------------------
            // Security Services
            // ---------------------------------------------------------
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ---------------------------------------------------------
            // Controllers + Swagger
            // ---------------------------------------------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Event Booking API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT token like: Bearer {your token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });



            });


            // CORS
            // ---------------------------------------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient", policy =>
                {
                    policy.WithOrigins("https://localhost:7117")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // ---------------------------------------------------------
            // JWT Authentication
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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Check cookie
                        if (context.Request.Cookies.TryGetValue("jwt", out var jwt))
                        {
                            context.Token = jwt;
                            return Task.CompletedTask;
                        }

                        // Check Authorization header
                        var header = context.Request.Headers.Authorization.ToString();
                        if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("Bearer "))
                        {
                            context.Token = header.Substring("Bearer ".Length).Trim();
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // ---------------------------------------------------------
            // Build app
            // ---------------------------------------------------------
            var app = builder.Build();

            // ---------------------------------------------------------
            // Development tools: Swagger
            // ---------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ---------------------------------------------------------
            // Middleware Pipeline
            // ---------------------------------------------------------
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowBlazorClient");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // ---------------------------------------------------------
            // Database Seeding
            // ---------------------------------------------------------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                db.Database.Migrate();

                // Seed roles
                if (!db.Roles.Any())
                {
                    db.Roles.AddRange(
                        new Role { Name = "Admin" },
                        new Role { Name = "Manager" },
                        new Role { Name = "User" }
                    );
                    db.SaveChanges();
                }

                var admin = db.Users.FirstOrDefault(u => u.RoleId == 1);
                if (admin == null)
                {
                    admin = new AdminUser
                    {
                        Username = "admin",
                        Email = "rajputronak0058@gmail.com",
                        PasswordHash = passwordHasher.HashPassword("Admin@123"),
                        RoleId = 1,
                        PhoneNumber = "9999999999",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    db.Users.Add(admin);
                }
                else
                {
                    admin.Email = "rajputronak0058@gmail.com";
                    admin.CreatedAt = DateTime.UtcNow;
                    admin.IsActive = true;
                    db.Users.Update(admin);
                }

                db.SaveChanges();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                await CountrySeeder.SeedAsync(db);
                await StateSeeder.SeedAsync(db);
            }

            // ---------------------------------------------------------
            // Run App
            // ---------------------------------------------------------
            app.Run();
        }
    }
}
