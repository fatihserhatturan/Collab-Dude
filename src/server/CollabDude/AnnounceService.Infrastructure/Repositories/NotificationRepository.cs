using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Where(x => x.Username == username && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Take(100) // Limit to last 100 notifications
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUsernameAsync(string username)
    {
        return await _dbSet
            .Where(x => x.Username == username && !x.IsRead && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Take(50) // Limit to last 50 unread notifications
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountByUsernameAsync(string username)
    {
        return await _dbSet
            .CountAsync(x => x.Username == username && !x.IsRead && !x.IsDeleted);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _dbSet.FindAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string username)
    {
        var unreadNotifications = await _dbSet
            .Where(x => x.Username == username && !x.IsRead && !x.IsDeleted)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
        }

        if (unreadNotifications.Any())
        {
            await _context.SaveChangesAsync();
        }
    }
}