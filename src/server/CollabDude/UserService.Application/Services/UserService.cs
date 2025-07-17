using AutoMapper;
using BCrypt.Net;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserRequestDto request)
        {
            // Validate unique constraints
            if (await _userRepository.IsEmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            if (await _userRepository.IsUsernameExistsAsync(request.UserName))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Get role by name
            var role = await _roleRepository.GetByNameAsync(request.RoleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{request.RoleName}' not found");
            }

            // Create user entity
            var user = _mapper.Map<User>(request);
            user.RoleId = role.Id;
            user.PasswordHash = HashPassword(request.Password);
            user.IsActive = true;

            // Save to database
            var createdUser = await _userRepository.AddAsync(user);
            
            // Load role for response
            var userWithRole = await _userRepository.GetByIdAsync(createdUser.Id);
            return _mapper.Map<UserDto>(userWithRole);
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserRequestDto request)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Check if email is being changed and if it's unique
            if (existingUser.Email != request.Email && await _userRepository.IsEmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Check if username is being changed and if it's unique
            if (existingUser.UserName != request.UserName && await _userRepository.IsUsernameExistsAsync(request.UserName))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Update properties
            existingUser.Email = request.Email;
            existingUser.UserName = request.UserName;
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.IsActive = request.IsActive;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<UserDto>(updatedUser);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _userRepository.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _userRepository.IsUsernameExistsAsync(username);
        }

        private string HashPassword(string password)
        {
            // For production, use a proper hashing library like BCrypt
            // This is just for demonstration
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}