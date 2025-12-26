using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Infrastructure.Integration.Alerts;
using Flownix.Backend.Infrastructure.Integration.Background;
using Flownix.Backend.Infrastructure.Integration.Reporting;
using Flownix.Backend.Infrastructure.Persistence.Services; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flownix.Backend.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<FlownixDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IFlownixDbContext>(provider =>
                provider.GetRequiredService<FlownixDbContext>());

            services.AddScoped<IAlertService, AlertService>();

            services.AddScoped<ISensorReadingCleanupService, SensorReadingCleanupService>();
            services.AddScoped<IPdfReportService, PdfReportService>();
            services.AddHostedService<SensorReadingCleanupWorker>();

            return services;
        }
    }
}