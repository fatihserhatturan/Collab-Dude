using AutoMapper;
using AnnounceService.Application.DTOs.Tag;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.Mappings;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        // Entity to DTO
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.AnnounceTags.Count));
        
        // DTO to Entity
        CreateMap<CreateTagRequestDto, Tag>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
    }
}