using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface IAnnounceRepository : IBaseRepository<Announce>
{
    Task<Announce?> GetByTitleAsync(string name);
    Task<bool> IsAnnounceActive(string name); 
    Task<IEnumerable<Announce>> GetAnnounceByCategory(string categoryName); 
}