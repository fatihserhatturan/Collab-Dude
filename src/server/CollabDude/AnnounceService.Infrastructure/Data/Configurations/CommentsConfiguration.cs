using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class CommentsConfiguration : BaseEntityConfiguration<Comments>
{
    public override void Configure(EntityTypeBuilder<Comments> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Comments");
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.AnnounceId)
            .IsRequired();
        
        builder.Property(x => x.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);
        
        // Indexes
        builder.HasIndex(x => x.AnnounceId)
            .HasDatabaseName("IX_Comments_AnnounceId");
        
        builder.HasIndex(x => x.Username)
            .HasDatabaseName("IX_Comments_Username");
        
        builder.HasIndex(x => x.ParentCommentId)
            .HasDatabaseName("IX_Comments_ParentCommentId");
        
        // Self-referencing relationship for replies
        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}