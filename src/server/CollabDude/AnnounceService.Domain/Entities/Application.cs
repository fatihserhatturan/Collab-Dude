using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Application : BaseEntity
{
    public Guid AnnounceId { get; set; }
    public string ApplicantUsername { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    
    public virtual Announce Announce { get; set; } = null!;
}

public enum ApplicationStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Withdrawn = 3
}