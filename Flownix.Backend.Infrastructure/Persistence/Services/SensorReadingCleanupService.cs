// Flownix.Backend.Infrastructure/Persistence/Services/SensorReadingCleanupService.cs
using Flownix.Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flownix.Backend.Infrastructure.Persistence.Services
{
    public class SensorReadingCleanupService : ISensorReadingCleanupService
    {
        private readonly IFlownixDbContext _context;
        private readonly ILogger<SensorReadingCleanupService> _logger;

        public SensorReadingCleanupService(
            IFlownixDbContext context,
            ILogger<SensorReadingCleanupService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CleanupOldReadingsAsync(TimeSpan maxAge, CancellationToken cancellationToken)
        {
            var threshold = DateTime.UtcNow - maxAge;

            _logger.LogInformation("Starting SensorReading cleanup, threshold: {Threshold}", threshold);

            var oldReadings = await _context.SensorReadings
                .Where(r => r.RecordedAt < threshold)
                .ToListAsync(cancellationToken);

            if (!oldReadings.Any())
            {
                _logger.LogInformation("No SensorReadings older than {Threshold} found.", threshold);
                return;
            }

            _context.SensorReadings.RemoveRange(oldReadings);

            var affected = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SensorReading cleanup completed. Deleted {Count} records.", affected);
        }
    }
}