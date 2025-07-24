using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface IAnnounceRepository : IBaseRepository<Announce>
{
    Task<Announce?> GetByTitleAsync(string title);
    Task<bool> IsAnnounceTitleExistsAsync(string title);
    Task<IEnumerable<Announce>> GetAnnouncesByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Announce>> GetAnnouncesByUsernameAsync(string username);
    Task<IEnumerable<Announce>> GetActiveAnnouncesAsync();
    Task<IEnumerable<Announce>> GetExpiredAnnouncesAsync();
    Task<IEnumerable<Announce>> GetAnnouncesByStatusAsync(AnnounceStatus status);
    Task<IEnumerable<Announce>> GetAnnouncesByCollaborationTypeAsync(CollaborationType type);
    Task<IEnumerable<Announce>> SearchAnnouncesAsync(string searchTerm);
    Task<Announce?> GetAnnounceWithDetailsAsync(Guid id);
}