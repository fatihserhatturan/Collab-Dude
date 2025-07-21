using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}