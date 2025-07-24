using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(x => x.Announces.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() && !x.IsDeleted);
    }

    public async Task<bool> IsCategoryNameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(x => x.Name.ToLower() == name.ToLower() && !x.IsDeleted);
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Include(x => x.Announces.Where(a => !a.IsDeleted))
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithAnnouncesAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Announces.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetAllAsync to include announces count and proper ordering
    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Announces.Where(a => !a.IsDeleted))
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }
}