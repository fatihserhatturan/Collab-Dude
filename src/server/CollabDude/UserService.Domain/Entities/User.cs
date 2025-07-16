using UserService.Domain.Common;

namespace UserService.Domain.Entities;

public class User : BaseEntity
{
    public User()
    {
        IsActive = true;
    }
    
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}