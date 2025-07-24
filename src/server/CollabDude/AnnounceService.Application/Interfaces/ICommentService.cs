using AnnounceService.Application.DTOs.Comments;

namespace AnnounceService.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto?> GetCommentByIdAsync(Guid id);
    Task<IEnumerable<CommentDto>> GetCommentsByAnnounceIdAsync(Guid announceId);
    Task<IEnumerable<CommentDto>> GetCommentsByUsernameAsync(string username);
    Task<CommentDto> CreateCommentAsync(CreateCommentRequestDto request, string username);
    Task<CommentDto> UpdateCommentAsync(UpdateCommentRequestDto request, string username);
    Task DeleteCommentAsync(Guid id, string username);
    Task<bool> IsCommentOwnerAsync(Guid commentId, string username);

}  