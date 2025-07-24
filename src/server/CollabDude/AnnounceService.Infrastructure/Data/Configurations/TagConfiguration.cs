using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class TagConfiguration : BaseEntityConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Tags");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasDefaultValue("#007bff");
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Name");
        
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Tags_IsActive");
    }
}