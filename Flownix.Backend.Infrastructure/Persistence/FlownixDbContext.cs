using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Flownix.Backend.Infrastructure.Persistence
{
    public class FlownixDbContext : DbContext, IFlownixDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<WaterObject> WaterObjects { get; set; }

        public DbSet<UserObjectAccess> UserObjectAccesses { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        public DbSet<SensorReading> SensorReadings { get; set; }

        public DbSet<Pump> Pumps { get; set; }

        public DbSet<Alert> Alerts { get; set; }

        public FlownixDbContext(DbContextOptions<FlownixDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FlownixDbContext).Assembly);

            modelBuilder.Entity<User>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<WaterObject>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<UserObjectAccess>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<Sensor>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<SensorReading>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<Pump>()
                .HasQueryFilter(e => e.DeletedAt == null);

            modelBuilder.Entity<Alert>()
                .HasQueryFilter(e => e.DeletedAt == null);

            base.OnModelCreating(modelBuilder);
        }
    }
}
