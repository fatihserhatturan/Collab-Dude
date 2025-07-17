using UserService.Application.DTOs.Common;

namespace UserService.Application.DTOs.Role;

public class RoleDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
}