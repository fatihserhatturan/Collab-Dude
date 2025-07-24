using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Notifications");
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(NotificationType.Info);
        
        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(x => x.RelatedEntityType)
            .HasMaxLength(50);
        
        builder.Property(x => x.ActionUrl)
            .HasMaxLength(500);
        
        // Indexes
        builder.HasIndex(x => x.Username)
            .HasDatabaseName("IX_Notifications_Username");
        
        builder.HasIndex(x => x.IsRead)
            .HasDatabaseName("IX_Notifications_IsRead");
        
        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_Notifications_Type");
        
        builder.HasIndex(x => new { x.Username, x.IsRead })
            .HasDatabaseName("IX_Notifications_Username_IsRead");
    }
}