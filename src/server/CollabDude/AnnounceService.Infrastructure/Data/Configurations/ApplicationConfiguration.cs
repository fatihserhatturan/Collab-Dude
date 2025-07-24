using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class ApplicationConfiguration : BaseEntityConfiguration<Domain.Entities.Application>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Applications");
        
        builder.Property(x => x.AnnounceId)
            .IsRequired();
        
        builder.Property(x => x.ApplicantUsername)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ApplicationStatus.Pending);
        
        builder.Property(x => x.ReviewNote)
            .HasMaxLength(500);
        
        // Indexes
        builder.HasIndex(x => x.AnnounceId)
            .HasDatabaseName("IX_Applications_AnnounceId");
        
        builder.HasIndex(x => x.ApplicantUsername)
            .HasDatabaseName("IX_Applications_ApplicantUsername");
        
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Applications_Status");
        
        // Unique constraint to prevent duplicate applications
        builder.HasIndex(x => new { x.AnnounceId, x.ApplicantUsername })
            .IsUnique()
            .HasDatabaseName("IX_Applications_AnnounceId_ApplicantUsername");
    }
}