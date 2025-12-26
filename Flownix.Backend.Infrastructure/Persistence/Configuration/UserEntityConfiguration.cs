using Flownix.Backend.Domain.Entities;
using Flownix.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Configuration
{
    internal class UserEntityConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(400);

            builder.Property(u => u.RefreshTokenExpiryTime);

            builder.Property(u => u.LastLoginAt);
        }
    }
}
