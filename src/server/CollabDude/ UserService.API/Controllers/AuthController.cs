using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if (result == null)
                {
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult("Invalid credentials"));
                }

                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken([FromBody] string token)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(token);
                if (result == null)
                {
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult("Invalid or expired token"));
                }

                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Token not found"));
                }

                await _authService.LogoutAsync(token);
                return Ok(ApiResponse<object>.SuccessResult(null, "Logged out successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] string token)
        {
            try
            {
                var isValid = await _authService.ValidateTokenAsync(token);
                return Ok(ApiResponse<bool>.SuccessResult(isValid));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }
    }
}