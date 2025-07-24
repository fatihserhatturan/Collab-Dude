using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface ICommentsRepository : IBaseRepository<Comments>
{
    Task<IEnumerable<Comments>> GetByUsernameAsync(string username);
    Task<IEnumerable<Comments>> GetByAnnounceIdAsync(Guid announceId);
    Task<IEnumerable<Comments>> GetRepliesByParentIdAsync(Guid parentCommentId);
    Task<Comments?> GetCommentWithRepliesAsync(Guid id);
}