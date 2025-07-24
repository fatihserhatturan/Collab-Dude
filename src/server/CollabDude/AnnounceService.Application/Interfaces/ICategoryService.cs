using AnnounceService.Application.DTOs.Category;

namespace AnnounceService.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<CategoryDto?> GetCategoryByNameAsync(string name);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request);
    Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequestDto request);
    Task DeleteCategoryAsync(Guid id);
    Task<bool> IsCategoryNameExistsAsync(string name);
}