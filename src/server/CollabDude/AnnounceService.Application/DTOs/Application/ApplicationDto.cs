using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.DTOs.Announce;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Application;

public class ApplicationDto : BaseDto
{
    public Guid AnnounceId { get; set; }
    public string ApplicantUsername { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    
    // Navigation properties
    public AnnounceDto? Announce { get; set; }
    
    public string StatusText => Status switch
    {
        ApplicationStatus.Pending => "Beklemede",
        ApplicationStatus.Approved => "Onaylandı",
        ApplicationStatus.Rejected => "Reddedildi",
        ApplicationStatus.Withdrawn => "Geri Çekildi",
        _ => "Bilinmiyor"
    };
}