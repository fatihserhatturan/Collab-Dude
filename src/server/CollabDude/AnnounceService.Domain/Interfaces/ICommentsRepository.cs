using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface ICommentsRepository : IBaseRepository<Comments>
{
    Task<Comments> GetByUsernameAsync(string username);
    Task<IEnumerable<Comments>> GetByAnnounceAsync(string username);
}