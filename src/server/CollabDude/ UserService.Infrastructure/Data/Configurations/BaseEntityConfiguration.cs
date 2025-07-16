using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Common;

namespace UserService.Infrastructure.Data.Configurations
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id)
                .IsRequired();
            
            builder.Property(x => x.CreatedAt)
                .IsRequired();
            
            builder.Property(x => x.UpdatedAt)
                .IsRequired();
            
            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            
            // Global query filter for soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}