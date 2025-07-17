using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Role;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roles));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<RoleDto>>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("with-users")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetRolesWithUsers()
        {
            try
            {
                var roles = await _roleService.GetRolesWithUsersAsync();
                return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roles));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<RoleDto>>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(Guid id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(ApiResponse<RoleDto>.ErrorResult("Role not found"));
                }
                return Ok(ApiResponse<RoleDto>.SuccessResult(role));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleByName(string name)
        {
            try
            {
                var role = await _roleService.GetRoleByNameAsync(name);
                if (role == null)
                {
                    return NotFound(ApiResponse<RoleDto>.ErrorResult("Role not found"));
                }
                return Ok(ApiResponse<RoleDto>.SuccessResult(role));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole(CreateRoleRequestDto request)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(request);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, 
                    ApiResponse<RoleDto>.SuccessResult(role, "Role created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(Guid id, UpdateRoleRequestDto request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse<RoleDto>.ErrorResult("ID mismatch"));
                }

                var role = await _roleService.UpdateRoleAsync(request);
                return Ok(ApiResponse<RoleDto>.SuccessResult(role, "Role updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole(Guid id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Role deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("check-name/{name}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckRoleNameExists(string name)
        {
            try
            {
                var exists = await _roleService.IsRoleNameExistsAsync(name);
                return Ok(ApiResponse<bool>.SuccessResult(exists));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }
    }
}