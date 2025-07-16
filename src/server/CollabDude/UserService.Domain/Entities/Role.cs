using UserService.Domain.Common;

namespace UserService.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        
        public static class RoleNames
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}