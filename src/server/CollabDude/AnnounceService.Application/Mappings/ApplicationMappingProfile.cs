using AutoMapper;
using AnnounceService.Application.DTOs.Application;

namespace AnnounceService.Application.Mappings;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // Entity to DTO
        CreateMap<Domain.Entities.Application, ApplicationDto>();
        
        // DTO to Entity
        CreateMap<CreateApplicationRequestDto, Domain.Entities.Application>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Entities.ApplicationStatus.Pending));
            
        CreateMap<UpdateApplicationStatusRequestDto, Domain.Entities.Application>()
            .ForMember(dest => dest.ReviewedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}