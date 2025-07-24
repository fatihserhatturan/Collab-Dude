using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Announce : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public AnnounceStatus Status { get; set; } = AnnounceStatus.Active;
    public DateTime? ExpiryDate { get; set; }
    public int MaxParticipants { get; set; } = 1;
    public int CurrentParticipants { get; set; } = 0;
    public string? Location { get; set; }
    public CollaborationType CollaborationType { get; set; } = CollaborationType.Project;
    public string? RequiredSkills { get; set; }
    public string? ContactInfo { get; set; }
    
    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    public virtual ICollection<AnnounceTag> AnnounceTags { get; set; } = new List<AnnounceTag>();
}

public enum AnnounceStatus
{
    Draft = 0,
    Active = 1,
    Closed = 2,
    Expired = 3,
    Suspended = 4
}

public enum CollaborationType
{
    Project = 0,
    Partnership = 1,
    Research = 2,
    Event = 3,
    Volunteer = 4,
    Other = 5
}