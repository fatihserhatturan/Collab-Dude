using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(UserServiceDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string name)  
        {
            return await _dbSet
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());  
        }

        public async Task<bool> IsRoleNameExistsAsync(string name)  
        {
            return await _dbSet.AnyAsync(x => x.Name.ToLower() == name.ToLower());  
        }

        public async Task<IEnumerable<Role>> GetRolesWithUsersAsync()
        {
            return await _dbSet.Include(x => x.Users).ToListAsync();  
        }
    }
}