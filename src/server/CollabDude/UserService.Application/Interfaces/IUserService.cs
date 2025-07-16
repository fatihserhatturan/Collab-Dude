using UserService.Application.DTOs.User;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserRequestDto request);
        Task<UserDto> UpdateUserAsync(UpdateUserRequestDto request);
        Task DeleteUserAsync(Guid id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
    }
}