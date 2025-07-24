using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Comments : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AnnounceId { get; set; }
    public Guid? ParentCommentId { get; set; } // Alt yorumlar için
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
    
    // Navigation properties
    public virtual Announce Announce { get; set; } = null!;
    public virtual Comments? ParentComment { get; set; }
    public virtual ICollection<Comments> Replies { get; set; } = new List<Comments>();
}