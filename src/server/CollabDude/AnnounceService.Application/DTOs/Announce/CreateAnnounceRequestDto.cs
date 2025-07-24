using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Announce;

public class CreateAnnounceRequestDto
{
    public string Title { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public int MaxParticipants { get; set; } = 1;
    public string? Location { get; set; }
    public CollaborationType CollaborationType { get; set; } = CollaborationType.Project;
    public string? RequiredSkills { get; set; }
    public string? ContactInfo { get; set; }
    public List<string> Tags { get; set; } = new();
}