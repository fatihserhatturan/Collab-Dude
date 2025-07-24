using AutoMapper;
using AnnounceService.Application.DTOs.Announce;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.Mappings;

public class AnnounceMappingProfile : Profile
{
    public AnnounceMappingProfile()
    {
        // Entity to DTO
        CreateMap<Announce, AnnounceDto>()
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.ApplicationsCount, opt => opt.MapFrom(src => src.Applications.Count));
            
        CreateMap<Announce, AnnounceDetailDto>()
            .IncludeBase<Announce, AnnounceDto>();
        
        // DTO to Entity
        CreateMap<CreateAnnounceRequestDto, Announce>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Entities.AnnounceStatus.Active))
            .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => 0));
            
        CreateMap<UpdateAnnounceRequestDto, Announce>();
    }
}