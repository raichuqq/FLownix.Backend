using Flownix.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Flownix.Backend.Application.Interfaces
{
    public interface IFlownixDbContext
    {
        DbSet<User> Users { get; }
        DbSet<WaterObject> WaterObjects { get; }

        DbSet<UserObjectAccess> UserObjectAccesses { get; }

        DbSet<Sensor> Sensors { get; }
        DbSet<SensorReading> SensorReadings { get; }

        DbSet<Pump> Pumps { get; }
        DbSet<Alert> Alerts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
