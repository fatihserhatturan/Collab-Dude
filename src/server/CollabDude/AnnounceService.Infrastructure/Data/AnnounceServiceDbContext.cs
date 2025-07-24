using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Common;
using AnnounceService.Infrastructure.Data.Configurations;

namespace AnnounceService.Infrastructure.Data;

public class AnnounceServiceDbContext : DbContext
{
    public AnnounceServiceDbContext(DbContextOptions<AnnounceServiceDbContext> options) : base(options)
    {
    }
    
    public DbSet<Announce> Announces { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Comments> Comments { get; set; } = null!;
    public DbSet<Domain.Entities.Application> Applications { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<AnnounceTag> AnnounceTags { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations
        modelBuilder.ApplyConfiguration(new AnnounceConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new CommentsConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new AnnounceTagConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        
        // Alternative: Apply all configurations in assembly
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnnounceServiceDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
        
        foreach (var entity in entities)
        {
            var baseEntity = (BaseEntity)entity.Entity;
            
            if (entity.State == EntityState.Added)
            {
                baseEntity.CreatedAt = DateTime.UtcNow;
            }
            
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
    }
}