using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class SensorReadingEntityConfiguration : BaseEntityConfiguration<SensorReading>
    {
        public override void Configure(EntityTypeBuilder<SensorReading> builder)
        {
            base.Configure(builder);

            builder.Property(r => r.Value)
                .IsRequired();

            builder.Property(r => r.RecordedAt)
                .IsRequired();

            builder.Property(r => r.SensorId)
                .IsRequired();

            builder.HasOne(r => r.Sensor)
                .WithMany(s => s.Readings)
                .HasForeignKey(r => r.SensorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
