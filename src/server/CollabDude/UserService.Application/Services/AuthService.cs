using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HashSet<string> _revokedTokens;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _revokedTokens = new HashSet<string>();
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Try to find user by email first, then by username
            var user = await _userRepository.GetByEmailAsync(request.EmailOrUsername) ??
                      await _userRepository.GetByUsernameAsync(request.EmailOrUsername);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                User = userDto
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || _revokedTokens.Contains(token))
            {
                return false;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(GetJwtSecretKey());
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = GetJwtIssuer(),
                    ValidateAudience = true,
                    ValidAudience = GetJwtAudience(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                
                // Check if user still exists and is active
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
                return user != null && user.IsActive;
            }
            catch
            {
                return false;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string token)
        {
            if (!await ValidateTokenAsync(token))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                // Revoke old token
                _revokedTokens.Add(token);

                // Generate new token
                var newToken = GenerateJwtToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                return new LoginResponseDto
                {
                    Token = newToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                    User = userDto
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            _revokedTokens.Add(token);
            return true;
        }

        private string GenerateJwtToken(Domain.Entities.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecretKey());
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience()
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GetJwtSecretKey()
        {
            return _configuration["JwtSettings:SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long";
        }

        private string GetJwtIssuer()
        {
            return _configuration["JwtSettings:Issuer"] ?? "UserService";
        }

        private string GetJwtAudience()
        {
            return _configuration["JwtSettings:Audience"] ?? "UserService";
        }

        private int GetTokenExpiryMinutes()
        {
            return int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");
        }
    }
}