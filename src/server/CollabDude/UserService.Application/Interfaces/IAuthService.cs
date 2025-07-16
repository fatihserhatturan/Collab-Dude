using UserService.Application.DTOs.Auth;

namespace UserService.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);  
    Task<bool> ValidateTokenAsync(string token);
    Task<LoginResponseDto?> RefreshTokenAsync(string token);   
    Task<bool> LogoutAsync(string token);
}