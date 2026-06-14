using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

            // ВРЕМЕННЫЙ обработчик ошибок.
            // Нужен, чтобы Swagger и Render Logs показали реальную причину ошибки 500.
            // После исправления Register/Login этот блок лучше убрать или заменить на нормальный production handler.
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    var exception = exceptionFeature?.Error;

                    Console.WriteLine("========== GLOBAL EXCEPTION ==========");
                    Console.WriteLine(exception?.ToString());
                    Console.WriteLine("======================================");

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = exception?.Message,
                        detail = exception?.ToString()
                    }));
                });
            });

            // На Render этот middleware может писать warning:
            // "Failed to determine the https port for redirect".
            // Пока оставляем, чтобы не менять лишнюю логику.
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flownix API v1");
                c.RoutePrefix = "swagger";
            });

            // ========== БЛОК МИГРАЦИЙ ==========
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    Console.WriteLine("🔄 Attempting to connect to database...");

                    var db = services.GetRequiredService<FlownixDbContext>();

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
                        Thread.Sleep(1000);
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
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("⚠️ Continuing without database migrations...");
                }
            }
            // ===================================

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.MapGet("/health", () =>
                Results.Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    service = "Flownix.Backend.API"
                }));

            Console.WriteLine("🚀 Application starting...");
            Console.WriteLine("📚 Swagger available at: /swagger");
            Console.WriteLine("🌐 Health check at: /health");

            app.Run();
        }
    }
}