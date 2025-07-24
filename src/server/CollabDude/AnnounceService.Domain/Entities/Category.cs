using AnnounceService.Domain.Common;

namespace AnnounceService.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    
    // Navigation properties
    public virtual ICollection<Announce> Announces { get; set; } = new List<Announce>();
    
    public static class CategoryNames
    {
        public const string Technology = "Technology";
        public const string Business = "Business";
        public const string Research = "Research";
        public const string Creative = "Creative";
        public const string Social = "Social";
        public const string Education = "Education";
        public const string Health = "Health";
        public const string Environment = "Environment";
    }
}