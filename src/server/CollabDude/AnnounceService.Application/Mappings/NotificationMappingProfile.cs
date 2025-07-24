using AutoMapper;
using AnnounceService.Application.DTOs.Notification;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        // Entity to DTO
        CreateMap<Notification, NotificationDto>();
        
        // DTO to Entity
        CreateMap<CreateNotificationRequestDto, Notification>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false));
    }
}