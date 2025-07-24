using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class AnnounceRepository : BaseRepository<Announce>, IAnnounceRepository
{
    public AnnounceRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<Announce?> GetByTitleAsync(string title)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.Comments)
            .Include(x => x.Applications)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(x => x.Title.ToLower() == title.ToLower() && !x.IsDeleted);
    }

    public async Task<bool> IsAnnounceTitleExistsAsync(string title)
    {
        return await _dbSet.AnyAsync(x => x.Title.ToLower() == title.ToLower() && !x.IsDeleted);
    }

    public async Task<IEnumerable<Announce>> GetAnnouncesByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> GetAnnouncesByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.Comments)
            .Include(x => x.Applications)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.Username == username && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> GetActiveAnnouncesAsync()
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.Status == AnnounceStatus.Active && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> GetExpiredAnnouncesAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(x => x.Category)
            .Where(x => x.ExpiryDate.HasValue && 
                       x.ExpiryDate.Value < now && 
                       x.Status == AnnounceStatus.Active && 
                       !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> GetAnnouncesByStatusAsync(AnnounceStatus status)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> GetAnnouncesByCollaborationTypeAsync(CollaborationType type)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.CollaborationType == type && 
                       x.Status == AnnounceStatus.Active && 
                       !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announce>> SearchAnnouncesAsync(string searchTerm)
    {
        var query = _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => x.Status == AnnounceStatus.Active && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(x => 
                x.Title.ToLower().Contains(lowerSearchTerm) ||
                (x.Description != null && x.Description.ToLower().Contains(lowerSearchTerm)) ||
                x.Content.ToLower().Contains(lowerSearchTerm) ||
                (x.RequiredSkills != null && x.RequiredSkills.ToLower().Contains(lowerSearchTerm)) ||
                x.Category.Name.ToLower().Contains(lowerSearchTerm) ||
                x.AnnounceTags.Any(at => at.Tag.Name.ToLower().Contains(lowerSearchTerm))
            );
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Announce?> GetAnnounceWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.Comments.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Replies.Where(r => !r.IsDeleted))
            .Include(x => x.Applications)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetByIdAsync to include Category by default
    public override async Task<Announce?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetAllAsync to include Category by default
    public override async Task<IEnumerable<Announce>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Category)
            .Include(x => x.AnnounceTags)
                .ThenInclude(at => at.Tag)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}