using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class TagRepository : BaseRepository<Tag>, ITagRepository
{
    public TagRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(x => x.AnnounceTags.Where(at => !at.IsDeleted))
            .ThenInclude(at => at.Announce)
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() && !x.IsDeleted);
    }

    public async Task<IEnumerable<Tag>> GetActiveTagsAsync()
    {
        return await _dbSet
            .Include(x => x.AnnounceTags.Where(at => !at.IsDeleted))
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetTagsByAnnounceIdAsync(Guid announceId)
    {
        return await _dbSet
            .Include(x => x.AnnounceTags.Where(at => !at.IsDeleted))
            .Where(x => x.AnnounceTags.Any(at => at.AnnounceId == announceId && !at.IsDeleted) && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    // Override GetAllAsync to include usage count and proper ordering
    public override async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.AnnounceTags.Where(at => !at.IsDeleted))
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}