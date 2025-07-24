namespace AnnounceService.Application.DTOs.Application;

public class CreateApplicationRequestDto
{
    public Guid AnnounceId { get; set; }
    public string Message { get; set; } = string.Empty;
}