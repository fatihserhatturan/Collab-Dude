using AutoMapper;
using AnnounceService.Application.DTOs.Announce;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class AnnounceService : IAnnounceService
{
    private readonly IAnnounceRepository _announceRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public AnnounceService(
        IAnnounceRepository announceRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _announceRepository = announceRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<AnnounceDto?> GetAnnounceByIdAsync(Guid id)
    {
        var announce = await _announceRepository.GetByIdAsync(id);
        return announce == null ? null : _mapper.Map<AnnounceDto>(announce);
    }

    public async Task<AnnounceDetailDto?> GetAnnounceDetailByIdAsync(Guid id)
    {
        var announce = await _announceRepository.GetAnnounceWithDetailsAsync(id);
        return announce == null ? null : _mapper.Map<AnnounceDetailDto>(announce);
    }

    public async Task<PagedResult<AnnounceDto>> GetAnnouncesAsync(AnnounceSearchDto searchDto)
    {
        var announces = await _announceRepository.SearchAnnouncesAsync(searchDto.SearchTerm ?? string.Empty);
        
        // Apply filters
        var filteredAnnounces = announces.AsQueryable();
        
        if (searchDto.CategoryId.HasValue)
        {
            filteredAnnounces = filteredAnnounces.Where(a => a.CategoryId == searchDto.CategoryId.Value);
        }
        
        if (searchDto.CollaborationType.HasValue)
        {
            filteredAnnounces = filteredAnnounces.Where(a => a.CollaborationType == searchDto.CollaborationType.Value);
        }
        
        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            filteredAnnounces = filteredAnnounces.Where(a => 
                a.Location != null && a.Location.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting
        filteredAnnounces = searchDto.SortBy.ToLower() switch
        {
            "title" => searchDto.SortDescending 
                ? filteredAnnounces.OrderByDescending(a => a.Title)
                : filteredAnnounces.OrderBy(a => a.Title),
            "createdat" => searchDto.SortDescending 
                ? filteredAnnounces.OrderByDescending(a => a.CreatedAt)
                : filteredAnnounces.OrderBy(a => a.CreatedAt),
            _ => filteredAnnounces.OrderByDescending(a => a.CreatedAt)
        };

        var totalCount = filteredAnnounces.Count();
        var items = filteredAnnounces
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        var mappedItems = _mapper.Map<List<AnnounceDto>>(items);
        
        return new PagedResult<AnnounceDto>(mappedItems, totalCount, searchDto.Page, searchDto.PageSize);
    }

    public async Task<IEnumerable<AnnounceDto>> GetAnnouncesByUsernameAsync(string username)
    {
        var announces = await _announceRepository.GetAnnouncesByUsernameAsync(username);
        return _mapper.Map<IEnumerable<AnnounceDto>>(announces);
    }

    public async Task<IEnumerable<AnnounceDto>> GetAnnouncesByCategoryAsync(Guid categoryId)
    {
        var announces = await _announceRepository.GetAnnouncesByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<AnnounceDto>>(announces);
    }

    public async Task<AnnounceDto> CreateAnnounceAsync(CreateAnnounceRequestDto request, string username)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Create announce
        var announce = _mapper.Map<Announce>(request);
        announce.Username = username;

        var createdAnnounce = await _announceRepository.AddAsync(announce);

        // Handle tags
        if (request.Tags.Any())
        {
            await HandleAnnounceTags(createdAnnounce.Id, request.Tags);
        }

        // Reload with relations
        var announceWithDetails = await _announceRepository.GetAnnounceWithDetailsAsync(createdAnnounce.Id);
        
        return _mapper.Map<AnnounceDto>(announceWithDetails);
    }

    public async Task<AnnounceDto> UpdateAnnounceAsync(UpdateAnnounceRequestDto request, string username)
    {
        var existingAnnounce = await _announceRepository.GetByIdAsync(request.Id);
        if (existingAnnounce == null)
        {
            throw new InvalidOperationException("Announce not found");
        }

        if (existingAnnounce.Username != username)
        {
            throw new UnauthorizedAccessException("You can only update your own announces");
        }

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Update properties
        _mapper.Map(request, existingAnnounce);
        existingAnnounce.UpdatedAt = DateTime.UtcNow;

        var updatedAnnounce = await _announceRepository.UpdateAsync(existingAnnounce);

        // Handle tags
        await HandleAnnounceTags(updatedAnnounce.Id, request.Tags);

        var announceWithDetails = await _announceRepository.GetAnnounceWithDetailsAsync(updatedAnnounce.Id);
        return _mapper.Map<AnnounceDto>(announceWithDetails);
    }

    public async Task DeleteAnnounceAsync(Guid id, string username)
    {
        var announce = await _announceRepository.GetByIdAsync(id);
        if (announce == null)
        {
            throw new InvalidOperationException("Announce not found");
        }

        if (announce.Username != username)
        {
            throw new UnauthorizedAccessException("You can only delete your own announces");
        }

        await _announceRepository.DeleteAsync(id);
    }

    public async Task<bool> IsAnnounceOwnerAsync(Guid announceId, string username)
    {
        var announce = await _announceRepository.GetByIdAsync(announceId);
        return announce?.Username == username;
    }

    public async Task<IEnumerable<AnnounceDto>> GetExpiredAnnouncesAsync()
    {
        var expiredAnnounces = await _announceRepository.GetExpiredAnnouncesAsync();
        return _mapper.Map<IEnumerable<AnnounceDto>>(expiredAnnounces);
    }

    public async Task MarkAsExpiredAsync(Guid announceId)
    {
        var announce = await _announceRepository.GetByIdAsync(announceId);
        if (announce != null && announce.Status != AnnounceStatus.Expired)
        {
            announce.Status = AnnounceStatus.Expired;
            announce.UpdatedAt = DateTime.UtcNow;
            await _announceRepository.UpdateAsync(announce);

            // Send notification to owner
            await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationRequestDto
            {
                Username = announce.Username,
                Title = "İlan Süresi Doldu",
                Message = $"'{announce.Title}' başlıklı ilanınızın süresi dolmuştur.",
                Type = NotificationType.AnnounceExpired,
                RelatedEntityId = announceId,
                RelatedEntityType = "Announce"
            });
        }
    }

    private async Task HandleAnnounceTags(Guid announceId, List<string> tagNames)
    {
        // This would typically involve a separate AnnounceTag repository
        // For now, we'll leave it as a placeholder
        foreach (var tagName in tagNames)
        {
            var tag = await _tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                await _tagRepository.AddAsync(new Tag { Name = tagName });
            }
        }
    }
}