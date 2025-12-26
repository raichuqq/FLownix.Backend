using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class AlertEntityConfiguration : BaseEntityConfiguration<Alert>
    {
        public override void Configure(EntityTypeBuilder<Alert> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.Type)
                .IsRequired();

            builder.Property(a => a.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.IsRead)
                .IsRequired();

            builder.HasOne(a => a.WaterObject)
                .WithMany(o => o.Alerts)
                .HasForeignKey(a => a.WaterObjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Sensor)
                .WithMany()
                .HasForeignKey(a => a.SensorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
