namespace AnnounceService.Application.DTOs.Comments;

public class UpdateCommentRequestDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}