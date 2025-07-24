using AnnounceService.Domain.Entities;

namespace AnnounceService.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<bool> IsCategoryNameExistsAsync(string name);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetCategoryWithAnnouncesAsync(Guid id);
}