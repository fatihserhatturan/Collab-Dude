using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface ITagRepository : IBaseRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetActiveTagsAsync();
    Task<IEnumerable<Tag>> GetTagsByAnnounceIdAsync(Guid announceId);
}