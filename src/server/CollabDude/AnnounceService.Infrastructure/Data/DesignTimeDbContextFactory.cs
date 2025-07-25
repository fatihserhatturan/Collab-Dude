using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AnnounceServiceDbContext>
{
    public AnnounceServiceDbContext CreateDbContext(string[] args)
    {
        // Basit bir connection string ile DbContext oluştur
        var connectionString = "Host=localhost;Database=announce_service_dev_db;Username=dev_user;Password=dev_password123";

        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<AnnounceServiceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AnnounceServiceDbContext(optionsBuilder.Options);
    }
}