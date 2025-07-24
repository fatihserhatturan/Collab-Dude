using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Infrastructure.Data.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Categories");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.IconUrl)
            .HasMaxLength(500);
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.Property(x => x.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Name");
        
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Categories_IsActive");
        
        builder.HasIndex(x => x.SortOrder)
            .HasDatabaseName("IX_Categories_SortOrder");
        
        // Seed data
        builder.HasData(
            new Category
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = Category.CategoryNames.Technology,
                Description = "Teknoloji ve yazılım projeleri",
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = Category.CategoryNames.Business,
                Description = "İş dünyası ve girişimcilik",
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = Category.CategoryNames.Research,
                Description = "Akademik araştırma ve bilim",
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = Category.CategoryNames.Creative,
                Description = "Yaratıcı projeler ve sanat",
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}