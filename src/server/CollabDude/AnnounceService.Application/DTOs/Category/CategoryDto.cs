using AnnounceService.Application.DTOs.Common;

namespace AnnounceService.Application.DTOs.Category;

public class CategoryDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public int AnnounceCount { get; set; }
}