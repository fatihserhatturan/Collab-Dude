using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class AnnounceTagConfiguration : BaseEntityConfiguration<AnnounceTag>
{
    public override void Configure(EntityTypeBuilder<AnnounceTag> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("AnnounceTags");
        
        builder.Property(x => x.AnnounceId)
            .IsRequired();
        
        builder.Property(x => x.TagId)
            .IsRequired();
        
        // Composite key
        builder.HasIndex(x => new { x.AnnounceId, x.TagId })
            .IsUnique()
            .HasDatabaseName("IX_AnnounceTags_AnnounceId_TagId");
        
        // Relationships
        builder.HasOne(x => x.Announce)
            .WithMany(x => x.AnnounceTags)
            .HasForeignKey(x => x.AnnounceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Tag)
            .WithMany(x => x.AnnounceTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}