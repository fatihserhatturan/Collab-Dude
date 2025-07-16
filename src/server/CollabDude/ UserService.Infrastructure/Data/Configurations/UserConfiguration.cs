using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService.Infrastructure.Data.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            
            builder.ToTable("Users");
            

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(x => x.RoleId)  
                .IsRequired();
            
            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
            
            builder.HasIndex(x => x.UserName)
                .IsUnique()
                .HasDatabaseName("IX_Users_UserName");
            
            // Relationship
            builder.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}