using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class WaterObjectEntityConfiguration : BaseEntityConfiguration<WaterObject>
    {
        public override void Configure(EntityTypeBuilder<WaterObject> builder)
        {
            base.Configure(builder);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(w => w.Location)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(w => w.Type)
                .IsRequired();

            builder.Property(w => w.MaxVolume)
                .IsRequired();

            builder.Property(w => w.CurrentVolume)
                .IsRequired();

            builder.Property(w => w.IsActive)
                .IsRequired();
        }
    }
}
