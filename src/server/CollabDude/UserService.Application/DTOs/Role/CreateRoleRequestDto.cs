﻿namespace UserService.Application.DTOs.Role;

public class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}