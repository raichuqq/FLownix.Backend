using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flownix.Backend.Infrastructure.Integration.Alerts
{
    public class AlertService : IAlertService
    {
        private readonly IFlownixDbContext _context;
        private readonly ILogger<AlertService> _logger;

        public AlertService(
            IFlownixDbContext context,
            ILogger<AlertService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckAndCreateAlertsAsync(
            SensorReading reading,
            CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"=== ALERT SERVICE CALLED ===");
                Console.WriteLine($"SensorId: {reading.SensorId}, Value: {reading.Value}");

                var sensor = await _context.Sensors
                    .Include(s => s.WaterObject)
                    .FirstOrDefaultAsync(s => s.Id == reading.SensorId, cancellationToken);

                if (sensor == null)
                {
                    Console.WriteLine($"ERROR: Sensor {reading.SensorId} not found!");
                    _logger.LogWarning($"Sensor {reading.SensorId} not found");
                    return;
                }

                Console.WriteLine($"Sensor found: Min={sensor.MinValue}, Max={sensor.MaxValue}, Active={sensor.IsActive}");

                if (!sensor.IsActive)
                {
                    Console.WriteLine($"WARNING: Sensor {sensor.Id} is not active");
                    _logger.LogWarning($"Sensor {sensor.Id} not active");
                    return;
                }

                var waterObject = sensor.WaterObject;
                if (waterObject == null)
                {
                    Console.WriteLine($"ERROR: WaterObject not found for sensor {sensor.Id}");
                    _logger.LogWarning($"WaterObject not found for sensor {sensor.Id}");
                    return;
                }

                Console.WriteLine($"Checking: {reading.Value} against range {sensor.MinValue}-{sensor.MaxValue}");

                var alerts = new List<Alert>();

                if (reading.Value < sensor.MinValue)
                {
                    Console.WriteLine($"CREATING ALERT: Value {reading.Value} < Min {sensor.MinValue}");
                    alerts.Add(CreateAlert(
                        waterObject,
                        sensor,
                        $"{sensor.Type}: значение {reading.Value} {sensor.Unit} ниже минимального " +
                        $"{sensor.MinValue} {sensor.Unit}",
                        reading.Value,
                        sensor.MinValue,
                        sensor.MaxValue
                    ));
                }
                else if (reading.Value > sensor.MaxValue)
                {
                    Console.WriteLine($"CREATING ALERT: Value {reading.Value} > Max {sensor.MaxValue}");
                    alerts.Add(CreateAlert(
                        waterObject,
                        sensor,
                        $"{sensor.Type}: значение {reading.Value} {sensor.Unit} выше максимального " +
                        $"{sensor.MaxValue} {sensor.Unit}",
                        reading.Value,
                        sensor.MinValue,
                        sensor.MaxValue
                    ));
                }
                else
                {
                    Console.WriteLine($"NO ALERT: Value {reading.Value} is within range");
                }

                if (alerts.Count > 0)
                {
                    Console.WriteLine($"Saving {alerts.Count} alerts to database...");
                    await _context.Alerts.AddRangeAsync(alerts, cancellationToken);
                    var saved = await _context.SaveChangesAsync(cancellationToken);
                    Console.WriteLine($"Saved {saved} alerts to database");
                    _logger.LogInformation($"Created {alerts.Count} alerts for sensor {sensor.Id}");
                }
                else
                {
                    Console.WriteLine("No alerts to create");
                }

                Console.WriteLine($"=== ALERT SERVICE COMPLETED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION in AlertService: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                _logger.LogError(ex, "Error in CheckAndCreateAlertsAsync");
            }
        }

        public async Task CheckPumpAndCreateAlertsAsync(
            Pump pump,
            CancellationToken cancellationToken)
        {
            var waterObject = await _context.WaterObjects
                .FirstOrDefaultAsync(wo => wo.Id == pump.WaterObjectId, cancellationToken);

            if (waterObject == null)
            {
                _logger.LogWarning($"WaterObject not found for pump {pump.Id}");
                return;
            }

            if (pump.Status == PumpStatus.Error)
            {
                var alert = new Alert
                {
                    WaterObjectId = pump.WaterObjectId,
                    WaterObject = waterObject, 
                    Message = $"Насос '{pump.Name}' в состоянии ERROR",
                    Type = AlertType.Critical,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                var recentSimilarAlert = await _context.Alerts
                    .AnyAsync(a => a.WaterObjectId == pump.WaterObjectId &&
                                  a.Message.Contains($"Насос '{pump.Name}'") &&
                                  a.CreatedAt > DateTime.UtcNow.AddHours(-1) &&
                                  !a.IsRead,
                        cancellationToken);

                if (!recentSimilarAlert)
                {
                    _context.Alerts.Add(alert);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation($"Created pump alert for pump {pump.Name}");
                }
            }
        }

        private Alert CreateAlert(
            WaterObject waterObject,
            Sensor sensor,
            string message,
            double currentValue,
            double minValue,
            double maxValue)
        {
            return new Alert
            {
                WaterObjectId = waterObject.Id,
                WaterObject = waterObject, 
                SensorId = sensor.Id,
                Sensor = sensor, 
                Message = message,
                Type = GetAlertTypeForValue(currentValue, minValue, maxValue),
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
        }

        private AlertType GetAlertTypeForValue(double value, double minValue, double maxValue)
        {
            var range = maxValue - minValue;
            if (range <= 0) return AlertType.Warning;

            var deviationFromMin = Math.Abs(value - minValue);
            var deviationFromMax = Math.Abs(value - maxValue);
            var maxDeviation = Math.Max(deviationFromMin, deviationFromMax);

            return maxDeviation > (range * 0.3)
                ? AlertType.Critical
                : AlertType.Warning;
        }

        private async Task CheckWaterLevelAlertsAsync(
            Sensor sensor,
            SensorReading reading,
            WaterObject waterObject,
            List<Alert> alerts,
            CancellationToken cancellationToken)
        {
            if (waterObject.MaxVolume <= 0) return;

            var percentage = (reading.Value / waterObject.MaxVolume) * 100;

            if (percentage > 90)
            {
                alerts.Add(new Alert
                {
                    WaterObjectId = waterObject.Id,
                    WaterObject = waterObject,
                    SensorId = sensor.Id,
                    Sensor = sensor,
                    Message = $"Уровень воды {percentage:F1}% близок к максимальному ({waterObject.MaxVolume})",
                    Type = AlertType.Warning,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }
            else if (percentage < 10)
            {
                alerts.Add(new Alert
                {
                    WaterObjectId = waterObject.Id,
                    WaterObject = waterObject,
                    SensorId = sensor.Id,
                    Sensor = sensor,
                    Message = $"Уровень воды {percentage:F1}% близок к минимальному",
                    Type = AlertType.Critical,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            if (waterObject.CurrentVolume > waterObject.MaxVolume)
            {
                alerts.Add(new Alert
                {
                    WaterObjectId = waterObject.Id,
                    WaterObject = waterObject,
                    Message = $"Текущий объем {waterObject.CurrentVolume} превышает максимальный {waterObject.MaxVolume}",
                    Type = AlertType.Critical,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }
        }
    }
}