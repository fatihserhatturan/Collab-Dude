using AutoMapper;
using AnnounceService.Application.DTOs.Comments;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.Mappings;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        // Entity to DTO
        CreateMap<Comments, CommentDto>();
        
        // DTO to Entity
        CreateMap<CreateCommentRequestDto, Comments>()
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false));
            
        CreateMap<UpdateCommentRequestDto, Comments>()
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.EditedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}