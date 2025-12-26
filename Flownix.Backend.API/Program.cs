using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Flownix.Backend.Application;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Infrastructure.Integration.Authentication;
using Flownix.Backend.Infrastructure.Persistence;

namespace Flownix.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUserContextService, UserContextService>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Flownix API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
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
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtKey = builder.Configuration["Jwt:Key"]!;
                    var issuer = builder.Configuration["Jwt:Issuer"]!;
                    var audience = builder.Configuration["Jwt:Audience"]!;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Всегда используем HTTPS на Render
            app.UseHttpsRedirection();

            // Настраиваем Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flownix API v1");
                c.RoutePrefix = "swagger"; // Явно указываем маршрут
            });

            // ========== ИСПРАВЛЕННЫЙ БЛОК МИГРАЦИЙ ==========
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    Console.WriteLine("🔄 Attempting to connect to database...");

                    var db = services.GetRequiredService<FlownixDbContext>();

                    // Ждем, пока база данных станет доступной (до 30 попыток)
                    bool canConnect = false;
                    for (int i = 0; i < 30; i++)
                    {
                        try
                        {
                            canConnect = db.Database.CanConnect();
                            if (canConnect)
                            {
                                Console.WriteLine("✅ Database connection successful!");
                                break;
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки подключения на первых попытках
                        }

                        Console.WriteLine($"⏳ Waiting for database... ({i + 1}/30)");
                        System.Threading.Thread.Sleep(1000);
                    }

                    if (canConnect)
                    {
                        Console.WriteLine("🔄 Applying database migrations...");
                        db.Database.Migrate();
                        Console.WriteLine("✅ Database migrations applied successfully!");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Could not connect to database. Skipping migrations.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ ERROR during database setup: {ex.Message}");
                    Console.WriteLine("⚠️ Continuing without database migrations...");
                    // НЕ выбрасываем исключение - продолжаем работу приложения
                }
            }
            // ==============================================

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Редирект с корня на Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"));

            // Health check endpoint
            app.MapGet("/health", () =>
                Results.Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    service = "Flownix.Backend.API"
                }));

            Console.WriteLine($"🚀 Application starting...");
            Console.WriteLine($"📚 Swagger available at: /swagger");
            Console.WriteLine($"🌐 Health check at: /health");

            app.Run();
        }
    }
}