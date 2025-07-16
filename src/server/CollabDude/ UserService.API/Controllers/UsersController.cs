using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Interfaces;
using UserService.Domain.Entities;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("test-data")]
        public async Task<ActionResult> CreateTestData()
        {
            // Test için admin role al
            var adminRole = await _roleRepository.GetByNameAsync("Admin");
            if (adminRole == null)
            {
                return BadRequest("Admin role not found");
            }

            // Test user oluştur
            var testUser = new User
            {
                Email = "admin@test.com",
                UserName = "admin",
                PasswordHash = "temp_hash_123", // Gerçek projede BCrypt kullanılacak
                FirstName = "Admin",
                LastName = "User",
                RoleId = adminRole.Id,
                IsActive = true
            };

            await _userRepository.AddAsync(testUser);
            return Ok(new { Message = "Test user created", User = testUser });
        }
    }
}