using AnnounceService.Application.DTOs.Application;
using AnnounceService.Application.DTOs.Comments;

namespace AnnounceService.Application.DTOs.Announce;

public class AnnounceDetailDto : AnnounceDto
{
    public List<CommentDto> Comments { get; set; } = new();
    public List<ApplicationDto> Applications { get; set; } = new();
}