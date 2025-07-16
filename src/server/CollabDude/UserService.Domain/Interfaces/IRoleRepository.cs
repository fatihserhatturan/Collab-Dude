using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<bool> IsRoleNameExistsAsync(string name); 
    Task<IEnumerable<Role>> GetRolesWithUsersAsync(); 
}