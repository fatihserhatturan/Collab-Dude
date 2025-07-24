using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Application;

public class UpdateApplicationStatusRequestDto
{
    public Guid Id { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? ReviewNote { get; set; }
}