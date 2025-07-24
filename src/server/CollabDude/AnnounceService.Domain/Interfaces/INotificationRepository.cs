using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUsernameAsync(string username);
    Task<IEnumerable<Notification>> GetUnreadByUsernameAsync(string username);
    Task<int> GetUnreadCountByUsernameAsync(string username);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(string username);
}