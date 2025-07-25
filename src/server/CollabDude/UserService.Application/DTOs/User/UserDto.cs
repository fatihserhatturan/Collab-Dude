﻿using UserService.Application.DTOs.Common;

namespace UserService.Application.DTOs.User
{
    public class UserDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}