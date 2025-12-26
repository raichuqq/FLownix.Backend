using Flownix.Backend.Domain.Entities;

namespace Flownix.Backend.Application.Interfaces
{
    public interface IAlertService
    {
        Task CheckAndCreateAlertsAsync(SensorReading reading, CancellationToken cancellationToken);
        Task CheckPumpAndCreateAlertsAsync(Pump pump, CancellationToken cancellationToken);
    }
}