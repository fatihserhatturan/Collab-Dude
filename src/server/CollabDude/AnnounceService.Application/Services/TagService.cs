using AutoMapper;
using AnnounceService.Application.DTOs.Tag;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public TagService(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<TagDto?> GetTagByIdAsync(Guid id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        return tag == null ? null : _mapper.Map<TagDto>(tag);
    }

    public async Task<TagDto?> GetTagByNameAsync(string name)
    {
        var tag = await _tagRepository.GetByNameAsync(name);
        return tag == null ? null : _mapper.Map<TagDto>(tag);
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<IEnumerable<TagDto>> GetActiveTagsAsync()
    {
        var tags = await _tagRepository.GetActiveTagsAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<IEnumerable<TagDto>> GetTagsByAnnounceIdAsync(Guid announceId)
    {
        var tags = await _tagRepository.GetTagsByAnnounceIdAsync(announceId);
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> CreateTagAsync(CreateTagRequestDto request)
    {
        // Check if tag already exists
        var existingTag = await _tagRepository.GetByNameAsync(request.Name);
        if (existingTag != null)
        {
            throw new InvalidOperationException("Tag already exists");
        }

        var tag = _mapper.Map<Tag>(request);
        var createdTag = await _tagRepository.AddAsync(tag);
        
        return _mapper.Map<TagDto>(createdTag);
    }

    public async Task<TagDto> GetOrCreateTagAsync(string name)
    {
        var existingTag = await _tagRepository.GetByNameAsync(name);
        if (existingTag != null)
        {
            return _mapper.Map<TagDto>(existingTag);
        }

        var newTag = new Tag 
        { 
            Name = name,
            Color = "#007bff",
            IsActive = true
        };
        
        var createdTag = await _tagRepository.AddAsync(newTag);
        return _mapper.Map<TagDto>(createdTag);
    }

    public async Task DeleteTagAsync(Guid id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
        {
            throw new InvalidOperationException("Tag not found");
        }

        await _tagRepository.DeleteAsync(id);
    }
}