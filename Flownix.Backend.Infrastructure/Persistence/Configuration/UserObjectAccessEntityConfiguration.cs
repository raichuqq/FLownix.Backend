using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class UserObjectAccessEntityConfiguration : BaseEntityConfiguration<UserObjectAccess>
    {
        public override void Configure(EntityTypeBuilder<UserObjectAccess> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.User)
                .WithMany(u => u.ObjectAccesses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.WaterObject)
                .WithMany(o => o.UserAccesses)
                .HasForeignKey(x => x.WaterObjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.WaterObjectId })
                .IsUnique();
        }
    }
}
