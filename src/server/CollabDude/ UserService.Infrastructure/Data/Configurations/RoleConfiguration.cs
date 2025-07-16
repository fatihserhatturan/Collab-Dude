using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);
            
            builder.ToTable("Roles");
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.Description)
                .HasMaxLength(200);
            
            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("IX_Roles_Name");
            
            // Seed data
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = Role.RoleNames.Admin,
                    Description = "Administrator role with full access",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = Role.RoleNames.User,
                    Description = "Standard user role",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}