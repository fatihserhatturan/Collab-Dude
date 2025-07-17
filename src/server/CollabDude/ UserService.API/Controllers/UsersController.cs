using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(users));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<UserDto>>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
                }
                return Ok(ApiResponse<UserDto>.SuccessResult(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
                }
                return Ok(ApiResponse<UserDto>.SuccessResult(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPost]
        [AllowAnonymous] // Allow user registration
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserRequestDto request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                    ApiResponse<UserDto>.SuccessResult(user, "User created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(Guid id, UpdateUserRequestDto request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse<UserDto>.ErrorResult("ID mismatch"));
                }

                var user = await _userService.UpdateUserAsync(request);
                return Ok(ApiResponse<UserDto>.SuccessResult(user, "User updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists(string email)
        {
            try
            {
                var exists = await _userService.IsEmailExistsAsync(email);
                return Ok(ApiResponse<bool>.SuccessResult(exists));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("check-username/{username}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUsernameExists(string username)
        {
            try
            {
                var exists = await _userService.IsUsernameExistsAsync(username);
                return Ok(ApiResponse<bool>.SuccessResult(exists));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }
    }
}