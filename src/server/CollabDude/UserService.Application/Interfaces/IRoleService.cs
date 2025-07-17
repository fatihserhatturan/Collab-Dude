using UserService.Application.DTOs.Role;

namespace UserService.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDto?> GetRoleByIdAsync(Guid id);
        Task<RoleDto?> GetRoleByNameAsync(string name);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>> GetRolesWithUsersAsync();
        Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request);
        Task<RoleDto> UpdateRoleAsync(UpdateRoleRequestDto request);
        Task DeleteRoleAsync(Guid id);
        Task<bool> IsRoleNameExistsAsync(string name);
    }
}