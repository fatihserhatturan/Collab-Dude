﻿namespace AnnounceService.Application.DTOs.Category;

public class CreateCategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int SortOrder { get; set; } = 0;
}