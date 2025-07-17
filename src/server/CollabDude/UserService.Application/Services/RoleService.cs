using AutoMapper;
using UserService.Application.DTOs.Role;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string name)
        {
            var role = await _roleRepository.GetByNameAsync(name);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<IEnumerable<RoleDto>> GetRolesWithUsersAsync()
        {
            var roles = await _roleRepository.GetRolesWithUsersAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request)
        {
            // Validate unique name
            if (await _roleRepository.IsRoleNameExistsAsync(request.Name))
            {
                throw new InvalidOperationException("Role name already exists");
            }

            var role = _mapper.Map<Role>(request);
            var createdRole = await _roleRepository.AddAsync(role);
            
            return _mapper.Map<RoleDto>(createdRole);
        }

        public async Task<RoleDto> UpdateRoleAsync(UpdateRoleRequestDto request)
        {
            var existingRole = await _roleRepository.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                throw new InvalidOperationException("Role not found");
            }

            // Check if name is being changed and if it's unique
            if (existingRole.Name != request.Name && await _roleRepository.IsRoleNameExistsAsync(request.Name))
            {
                throw new InvalidOperationException("Role name already exists");
            }

            existingRole.Name = request.Name;
            existingRole.Description = request.Description;

            var updatedRole = await _roleRepository.UpdateAsync(existingRole);
            return _mapper.Map<RoleDto>(updatedRole);
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new InvalidOperationException("Role not found");
            }

            // Check if role has users
            var roleWithUsers = await _roleRepository.GetByNameAsync(role.Name);
            if (roleWithUsers != null && roleWithUsers.Users.Any())
            {
                throw new InvalidOperationException("Cannot delete role that has users assigned");
            }

            await _roleRepository.DeleteAsync(id);
        }

        public async Task<bool> IsRoleNameExistsAsync(string name)
        {
            return await _roleRepository.IsRoleNameExistsAsync(name);
        }
    }
}