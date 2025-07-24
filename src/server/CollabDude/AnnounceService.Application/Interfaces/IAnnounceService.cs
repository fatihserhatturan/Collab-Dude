using AnnounceService.Application.DTOs.Announce;
using AnnounceService.Application.DTOs.Common;

namespace AnnounceService.Application.Interfaces;

public interface IAnnounceService
{
    Task<AnnounceDto?> GetAnnounceByIdAsync(Guid id);
    Task<AnnounceDetailDto?> GetAnnounceDetailByIdAsync(Guid id);
    Task<PagedResult<AnnounceDto>> GetAnnouncesAsync(AnnounceSearchDto searchDto);
    Task<IEnumerable<AnnounceDto>> GetAnnouncesByUsernameAsync(string username);
    Task<IEnumerable<AnnounceDto>> GetAnnouncesByCategoryAsync(Guid categoryId);
    Task<AnnounceDto> CreateAnnounceAsync(CreateAnnounceRequestDto request, string username);
    Task<AnnounceDto> UpdateAnnounceAsync(UpdateAnnounceRequestDto request, string username);
    Task DeleteAnnounceAsync(Guid id, string username);
    Task<bool> IsAnnounceOwnerAsync(Guid announceId, string username);
    Task<IEnumerable<AnnounceDto>> GetExpiredAnnouncesAsync();
    Task MarkAsExpiredAsync(Guid announceId);
}