using Flownix.Backend.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flownix.Backend.Infrastructure.BackgroundServices
{
    public class SensorReadingCleanupBackgroundService : BackgroundService
    {
        private readonly ILogger<SensorReadingCleanupBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(1); 
        private readonly TimeSpan _maxAge = TimeSpan.FromDays(30); 

        public SensorReadingCleanupBackgroundService(
            ILogger<SensorReadingCleanupBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SensorReading Cleanup Background Service started");

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting scheduled SensorReading cleanup");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var cleanupService = scope.ServiceProvider
                            .GetRequiredService<ISensorReadingCleanupService>();

                        await cleanupService.CleanupOldReadingsAsync(_maxAge, stoppingToken);
                    }

                    _logger.LogInformation("Scheduled SensorReading cleanup completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during SensorReading cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SensorReading Cleanup Background Service stopped");
            await base.StopAsync(cancellationToken);
        }
    }
}