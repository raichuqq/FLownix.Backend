using Flownix.Backend.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flownix.Backend.Infrastructure.Integration.Background
{
    public class SensorReadingCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SensorReadingCleanupWorker> _logger;
        private readonly TimeSpan _interval;
        private readonly TimeSpan _maxAge;

        public SensorReadingCleanupWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<SensorReadingCleanupWorker> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var intervalHoursStr = configuration["SensorReadingCleanup:IntervalHours"];
            var maxAgeDaysStr = configuration["SensorReadingCleanup:MaxAgeDays"];

            var intervalHours = !string.IsNullOrEmpty(intervalHoursStr)
                ? int.Parse(intervalHoursStr) : 1; 
            var maxAgeDays = !string.IsNullOrEmpty(maxAgeDaysStr)
                ? int.Parse(maxAgeDaysStr) : 7; 

            _interval = TimeSpan.FromHours(intervalHours);
            _maxAge = TimeSpan.FromDays(maxAgeDays);

            _logger.LogInformation("SensorReadingCleanupWorker configured: " +
                                  $"Interval = {_interval.TotalHours}h, " +
                                  $"MaxAge = {_maxAge.TotalDays}d");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SensorReadingCleanupWorker started. " +
                                  $"Interval: {_interval}, MaxAge: {_maxAge}");

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting SensorReading cleanup...");

                    using var scope = _scopeFactory.CreateScope();
                    var cleanupService = scope.ServiceProvider
                        .GetRequiredService<ISensorReadingCleanupService>();

                    await cleanupService.CleanupOldReadingsAsync(_maxAge, stoppingToken);

                    _logger.LogInformation("SensorReading cleanup completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during SensorReading cleanup");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
            }

            _logger.LogInformation("SensorReadingCleanupWorker is stopping");
        }
    }
}