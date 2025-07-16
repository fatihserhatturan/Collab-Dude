using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(UserServiceDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Include(x => x.Role)
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId)
        {
            return await _dbSet
                .Include(x => x.Role)
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
        }

        // Override GetByIdAsync to include Role
        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Override GetAllAsync to include Role
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Role)
                .ToListAsync();
        }
    }
}