using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<AnnounceTag> AnnounceTags { get; set; } = new List<AnnounceTag>();
}

public class AnnounceTag : BaseEntity
{
    public Guid AnnounceId { get; set; }
    public Guid TagId { get; set; }
    
    // Navigation properties
    public virtual Announce Announce { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}