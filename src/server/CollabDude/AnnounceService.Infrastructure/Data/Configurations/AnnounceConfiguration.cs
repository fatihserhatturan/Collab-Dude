using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class AnnounceConfiguration : BaseEntityConfiguration<Announce>
{
    public override void Configure(EntityTypeBuilder<Announce> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Announces");
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(5000);
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(AnnounceStatus.Active);
        
        builder.Property(x => x.CollaborationType)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(CollaborationType.Project);
        
        builder.Property(x => x.MaxParticipants)
            .IsRequired()
            .HasDefaultValue(1);
        
        builder.Property(x => x.CurrentParticipants)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(x => x.Location)
            .HasMaxLength(200);
        
        builder.Property(x => x.RequiredSkills)
            .HasMaxLength(1000);
        
        builder.Property(x => x.ContactInfo)
            .HasMaxLength(500);
        
        builder.Property(x => x.CategoryId)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(x => x.Title)
            .HasDatabaseName("IX_Announces_Title");
        
        builder.HasIndex(x => x.Username)
            .HasDatabaseName("IX_Announces_Username");
        
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Announces_Status");
        
        builder.HasIndex(x => x.CategoryId)
            .HasDatabaseName("IX_Announces_CategoryId");
        
        builder.HasIndex(x => x.ExpiryDate)
            .HasDatabaseName("IX_Announces_ExpiryDate");
        
        builder.HasIndex(x => x.CollaborationType)
            .HasDatabaseName("IX_Announces_CollaborationType");
        
        // Relationships
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Announces)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Announce)
            .HasForeignKey(x => x.AnnounceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Applications)
            .WithOne(x => x.Announce)
            .HasForeignKey(x => x.AnnounceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.AnnounceTags)
            .WithOne(x => x.Announce)
            .HasForeignKey(x => x.AnnounceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}