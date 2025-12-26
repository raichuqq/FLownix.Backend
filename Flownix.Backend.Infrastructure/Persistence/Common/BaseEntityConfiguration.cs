using Flownix.Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flownix.Backend.Infrastructure.Persistence.Common
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAt)
                .IsRequired();
            builder.Property(e => e.UpdatedAt);
        }
    }
}