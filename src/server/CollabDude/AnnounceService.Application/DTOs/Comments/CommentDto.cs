using AnnounceService.Application.DTOs.Common;

namespace AnnounceService.Application.DTOs.Comments;

public class CommentDto : BaseDto
{
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AnnounceId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
    public int RepliesCount => Replies?.Count ?? 0;
}