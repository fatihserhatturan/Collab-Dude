using AnnounceService.Application.DTOs.Common;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Notification;

public class NotificationDto : BaseDto
{
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? ActionUrl { get; set; }
    
    public string TypeText => Type switch
    {
        NotificationType.Info => "Bilgi",
        NotificationType.Success => "Başarılı",
        NotificationType.Warning => "Uyarı",
        NotificationType.Error => "Hata",
        NotificationType.NewApplication => "Yeni Başvuru",
        NotificationType.ApplicationStatusChanged => "Başvuru Durumu Değişti",
        NotificationType.NewComment => "Yeni Yorum",
        NotificationType.AnnounceExpired => "İlan Süresi Doldu",
        _ => "Bilinmiyor"
    };
    
    public string TypeIcon => Type switch
    {
        NotificationType.Info => "info",
        NotificationType.Success => "check-circle",
        NotificationType.Warning => "alert-triangle",
        NotificationType.Error => "x-circle",
        NotificationType.NewApplication => "user-plus",
        NotificationType.ApplicationStatusChanged => "refresh-cw",
        NotificationType.NewComment => "message-circle",
        NotificationType.AnnounceExpired => "clock",
        _ => "bell"
    };
}