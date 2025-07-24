namespace AnnounceService.Application.DTOs.Comments;

public class CreateCommentRequestDto
{
    public Guid AnnounceId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}