using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class SensorEntityConfiguration : BaseEntityConfiguration<Sensor>
    {
        public override void Configure(EntityTypeBuilder<Sensor> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.Type)
                .IsRequired();

            builder.Property(s => s.MinValue)
                .IsRequired();

            builder.Property(s => s.MaxValue)
                .IsRequired();

            builder.Property(s => s.Unit)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.HasOne(s => s.WaterObject)
                .WithMany(o => o.Sensors)
                .HasForeignKey(s => s.WaterObjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
