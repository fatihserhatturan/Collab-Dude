namespace UserService.Application.DTOs.Role;

public class UpdateRoleRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}