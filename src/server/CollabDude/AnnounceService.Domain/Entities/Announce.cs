using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Announce : BaseEntity
{
    public required string Title { get; set; }
    public required Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public required string Content { get; set; }
    public required string Username { get; set; }
    
    public virtual Category Category { get; set; }
    
}