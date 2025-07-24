using AnnounceService.Application.DTOs.Tag;

namespace AnnounceService.Application.Interfaces;

public interface ITagService
{
    Task<TagDto?> GetTagByIdAsync(Guid id);
    Task<TagDto?> GetTagByNameAsync(string name);
    Task<IEnumerable<TagDto>> GetAllTagsAsync();
    Task<IEnumerable<TagDto>> GetActiveTagsAsync();
    Task<IEnumerable<TagDto>> GetTagsByAnnounceIdAsync(Guid announceId);
    Task<TagDto> CreateTagAsync(CreateTagRequestDto request);
    Task<TagDto> GetOrCreateTagAsync(string name);
    Task DeleteTagAsync(Guid id);
}