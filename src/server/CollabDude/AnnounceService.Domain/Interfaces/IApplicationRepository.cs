using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface IApplicationRepository : IBaseRepository<Application>
{
    Task<IEnumerable<Application>> GetByAnnounceIdAsync(Guid announceId);
    Task<IEnumerable<Application>> GetByUsernameAsync(string username);
    Task<IEnumerable<Application>> GetByStatusAsync(ApplicationStatus status);
    Task<Application?> GetByAnnounceAndUsernameAsync(Guid announceId, string username);
    Task<bool> HasUserAppliedAsync(Guid announceId, string username);
}