using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class CommentsRepository : BaseRepository<Comments>, ICommentsRepository
{
    public CommentsRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comments>> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Include(x => x.Replies.Where(r => !r.IsDeleted))
            .Where(x => x.Username == username && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comments>> GetByAnnounceIdAsync(Guid announceId)
    {
        return await _dbSet
            .Include(x => x.Replies.Where(r => !r.IsDeleted))
            .Where(x => x.AnnounceId == announceId && 
                       x.ParentCommentId == null && // Only top-level comments
                       !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comments>> GetRepliesByParentIdAsync(Guid parentCommentId)
    {
        return await _dbSet
            .Include(x => x.Replies.Where(r => !r.IsDeleted))
            .Where(x => x.ParentCommentId == parentCommentId && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comments?> GetCommentWithRepliesAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Include(x => x.Replies.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.Replies.Where(rr => !rr.IsDeleted))
            .Include(x => x.ParentComment)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetByIdAsync to include basic relations
    public override async Task<Comments?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Announce)
            .Include(x => x.ParentComment)
            .Include(x => x.Replies.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetAllAsync to include basic relations
    public override async Task<IEnumerable<Comments>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Announce)
            .Include(x => x.ParentComment)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}