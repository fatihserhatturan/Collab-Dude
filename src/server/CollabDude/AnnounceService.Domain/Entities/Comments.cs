using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Comments : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AnnounceId { get; set; }
    
    public virtual Announce Announce { get; set; }
}