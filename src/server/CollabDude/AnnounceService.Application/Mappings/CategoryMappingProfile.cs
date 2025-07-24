using AutoMapper;
using AnnounceService.Application.DTOs.Category;
using AnnounceService.Domain.Entities;

namespace AnnounceService.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Entity to DTO
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.AnnounceCount, opt => opt.MapFrom(src => src.Announces.Count));
        
        // DTO to Entity
        CreateMap<CreateCategoryRequestDto, Category>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            
        CreateMap<UpdateCategoryRequestDto, Category>();
    }
}