using AutoMapper;
using AnnounceService.Application.DTOs.Comments;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IAnnounceRepository _announceRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentsRepository commentsRepository,
        IAnnounceRepository announceRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _commentsRepository = commentsRepository;
        _announceRepository = announceRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _commentsRepository.GetByIdAsync(id);
        return comment == null ? null : _mapper.Map<CommentDto>(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByAnnounceIdAsync(Guid announceId)
    {
        var comments = await _commentsRepository.GetByAnnounceIdAsync(announceId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByUsernameAsync(string username)
    {
        var comments = await _commentsRepository.GetByUsernameAsync(username);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentRequestDto request, string username)
    {
        // Check if announce exists
        var announce = await _announceRepository.GetByIdAsync(request.AnnounceId);
        if (announce == null)
        {
            throw new InvalidOperationException("Announce not found");
        }

        // Check if parent comment exists (if replying to a comment)
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _commentsRepository.GetByIdAsync(request.ParentCommentId.Value);
            if (parentComment == null)
            {
                throw new InvalidOperationException("Parent comment not found");
            }

            if (parentComment.AnnounceId != request.AnnounceId)
            {
                throw new InvalidOperationException("Parent comment does not belong to the specified announce");
            }
        }

        // Create comment
        var comment = _mapper.Map<Comments>(request);
        comment.Username = username;

        var createdComment = await _commentsRepository.AddAsync(comment);

        // Send notification to announce owner (if not commenting on own announce)
        if (announce.Username != username)
        {
            await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationRequestDto
            {
                Username = announce.Username,
                Title = "Yeni Yorum",
                Message = $"{username} kullanıcısı '{announce.Title}' ilanınıza yorum yaptı.",
                Type = NotificationType.NewComment,
                RelatedEntityId = createdComment.Id,
                RelatedEntityType = "Comment"
            });
        }

        // Send notification to parent comment owner (if replying and not replying to own comment)
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _commentsRepository.GetByIdAsync(request.ParentCommentId.Value);
            if (parentComment != null && parentComment.Username != username)
            {
                await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationRequestDto
                {
                    Username = parentComment.Username,
                    Title = "Yorumunuza Yanıt",
                    Message = $"{username} kullanıcısı yorumunuza yanıt verdi.",
                    Type = NotificationType.NewComment,
                    RelatedEntityId = createdComment.Id,
                    RelatedEntityType = "Comment"
                });
            }
        }

        return _mapper.Map<CommentDto>(createdComment);
    }

    public async Task<CommentDto> UpdateCommentAsync(UpdateCommentRequestDto request, string username)
    {
        var existingComment = await _commentsRepository.GetByIdAsync(request.Id);
        if (existingComment == null)
        {
            throw new InvalidOperationException("Comment not found");
        }

        if (existingComment.Username != username)
        {
            throw new UnauthorizedAccessException("You can only update your own comments");
        }

        // Update properties
        existingComment.Content = request.Content;
        existingComment.IsEdited = true;
        existingComment.EditedAt = DateTime.UtcNow;
        existingComment.UpdatedAt = DateTime.UtcNow;

        var updatedComment = await _commentsRepository.UpdateAsync(existingComment);
        return _mapper.Map<CommentDto>(updatedComment);
    }

    public async Task DeleteCommentAsync(Guid id, string username)
    {
        var comment = await _commentsRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new InvalidOperationException("Comment not found");
        }

        if (comment.Username != username)
        {
            throw new UnauthorizedAccessException("You can only delete your own comments");
        }

        // Check if comment has replies
        var replies = await _commentsRepository.GetRepliesByParentIdAsync(id);
        if (replies.Any())
        {
            // Soft delete: mark as deleted but keep for reply structure
            comment.Content = "[Bu yorum silinmiştir]";
            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentsRepository.UpdateAsync(comment);
        }
        else
        {
            // Hard delete if no replies
            await _commentsRepository.DeleteAsync(id);
        }
    }

    public async Task<bool> IsCommentOwnerAsync(Guid commentId, string username)
    {
        var comment = await _commentsRepository.GetByIdAsync(commentId);
        return comment?.Username == username;
    }
}