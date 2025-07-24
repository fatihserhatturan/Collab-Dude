using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.DTOs.Category;
using AnnounceService.Application.DTOs.Tag;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Announce;

public class AnnounceDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public AnnounceStatus Status { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public string? Location { get; set; }
    public CollaborationType CollaborationType { get; set; }
    public string? RequiredSkills { get; set; }
    public string? ContactInfo { get; set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool IsFull => CurrentParticipants >= MaxParticipants;
    
    // Navigation properties
    public CategoryDto Category { get; set; } = null!;
    public List<TagDto> Tags { get; set; } = new();
    public int CommentsCount { get; set; }
    public int ApplicationsCount { get; set; }
}