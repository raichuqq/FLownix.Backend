using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class PumpEntityConfiguration : BaseEntityConfiguration<Pump>
    {
        public override void Configure(EntityTypeBuilder<Pump> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.LastUpdated);

            builder.HasOne(p => p.WaterObject)
                .WithMany(o => o.Pumps)
                .HasForeignKey(p => p.WaterObjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
