using AnnounceService.Application.DTOs.Common;

namespace AnnounceService.Application.DTOs.Tag;

public class TagDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UsageCount { get; set; }
}