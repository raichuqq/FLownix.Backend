using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using MediatR;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Application;
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

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MedFront API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен в формате: Bearer {token}"
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

            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUserContextService, UserContextService>();

            // JWT Authentication
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwtKey = builder.Configuration["Jwt:Key"];
                    if (string.IsNullOrWhiteSpace(jwtKey))
                        throw new InvalidOperationException("Jwt:Key is missing in configuration.");

                    var issuer = builder.Configuration["Jwt:Issuer"];
                    var audience = builder.Configuration["Jwt:Audience"];

                    if (string.IsNullOrWhiteSpace(issuer))
                        throw new InvalidOperationException("Jwt:Issuer is missing in configuration.");
                    if (string.IsNullOrWhiteSpace(audience))
                        throw new InvalidOperationException("Jwt:Audience is missing in configuration.");

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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}