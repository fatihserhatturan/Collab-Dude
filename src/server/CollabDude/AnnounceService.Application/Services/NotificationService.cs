using AutoMapper;
using AnnounceService.Application.DTOs.Notification;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        return notification == null ? null : _mapper.Map<NotificationDto>(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsByUsernameAsync(string username)
    {
        var notifications = await _notificationRepository.GetByUsernameAsync(username);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications.OrderByDescending(n => n.CreatedAt));
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByUsernameAsync(string username)
    {
        var notifications = await _notificationRepository.GetUnreadByUsernameAsync(username);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications.OrderByDescending(n => n.CreatedAt));
    }

    public async Task<int> GetUnreadCountByUsernameAsync(string username)
    {
        return await _notificationRepository.GetUnreadCountByUsernameAsync(username);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequestDto request)
    {
        var notification = _mapper.Map<Notification>(request);
        var createdNotification = await _notificationRepository.AddAsync(notification);
        
        return _mapper.Map<NotificationDto>(createdNotification);
    }

    public async Task MarkAsReadAsync(Guid notificationId, string username)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
        {
            throw new InvalidOperationException("Notification not found");
        }

        if (notification.Username != username)
        {
            throw new UnauthorizedAccessException("You can only mark your own notifications as read");
        }

        if (!notification.IsRead)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
        }
    }

    public async Task MarkAllAsReadAsync(string username)
    {
        await _notificationRepository.MarkAllAsReadAsync(username);
    }

    public async Task DeleteNotificationAsync(Guid id, string username)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
        {
            throw new InvalidOperationException("Notification not found");
        }

        if (notification.Username != username)
        {
            throw new UnauthorizedAccessException("You can only delete your own notifications");
        }

        await _notificationRepository.DeleteAsync(id);
    }
}