using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Notification : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; } 
    public string? ActionUrl { get; set; }
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    NewApplication = 4,
    ApplicationStatusChanged = 5,
    NewComment = 6,
    AnnounceExpired = 7
}