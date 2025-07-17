using AutoMapper;
using UserService.Application.DTOs.Role;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            // Entity to DTO
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count));
            
            // DTO to Entity
            CreateMap<CreateRoleRequestDto, Role>();
            CreateMap<UpdateRoleRequestDto, Role>();
        }
    }
}