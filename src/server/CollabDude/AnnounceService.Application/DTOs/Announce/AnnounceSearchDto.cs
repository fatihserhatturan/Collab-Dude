using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.DTOs.Announce;

public class AnnounceSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public CollaborationType? CollaborationType { get; set; }
    public string? Location { get; set; }
    public bool? IsRemote { get; set; }
    public List<string> Tags { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}