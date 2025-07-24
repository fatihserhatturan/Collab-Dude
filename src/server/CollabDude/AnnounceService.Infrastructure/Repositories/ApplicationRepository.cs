using Microsoft.EntityFrameworkCore;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;
using AnnounceService.Infrastructure.Data;

namespace AnnounceService.Infrastructure.Repositories;

public class ApplicationRepository : BaseRepository<Domain.Entities.Application>, IApplicationRepository
{
    public ApplicationRepository(AnnounceServiceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Entities.Application>> GetByAnnounceIdAsync(Guid announceId)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Where(x => x.AnnounceId == announceId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Domain.Entities.Application>> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Where(x => x.ApplicantUsername == username && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Domain.Entities.Application>> GetByStatusAsync(ApplicationStatus status)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Application?> GetByAnnounceAndUsernameAsync(Guid announceId, string username)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(x => x.AnnounceId == announceId && 
                                    x.ApplicantUsername == username && 
                                    !x.IsDeleted);
    }

    public async Task<bool> HasUserAppliedAsync(Guid announceId, string username)
    {
        return await _dbSet.AnyAsync(x => x.AnnounceId == announceId && 
                                        x.ApplicantUsername == username && 
                                        !x.IsDeleted);
    }

    // Override GetByIdAsync to include Announce by default
    public override async Task<Domain.Entities.Application?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    // Override GetAllAsync to include Announce by default
    public override async Task<IEnumerable<Domain.Entities.Application>> GetAllAsync()
    {
        return await _dbSet
            .Include(x => x.Announce)
                .ThenInclude(a => a.Category)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}