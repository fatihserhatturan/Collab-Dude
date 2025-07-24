using AnnounceService.Application.DTOs.Notification;

namespace AnnounceService.Application.Interfaces;

public interface INotificationService
{
    Task<NotificationDto?> GetNotificationByIdAsync(Guid id);
    Task<IEnumerable<NotificationDto>> GetNotificationsByUsernameAsync(string username);
    Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByUsernameAsync(string username);
    Task<int> GetUnreadCountByUsernameAsync(string username);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequestDto request);
    Task MarkAsReadAsync(Guid notificationId, string username);
    Task MarkAllAsReadAsync(string username);
    Task DeleteNotificationAsync(Guid id, string username);
}