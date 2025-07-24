// CategoryService.cs
using AutoMapper;
using AnnounceService.Application.DTOs.Category;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _categoryRepository.GetByNameAsync(name);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request)
    {
        // Validate unique name
        if (await _categoryRepository.IsCategoryNameExistsAsync(request.Name))
        {
            throw new InvalidOperationException("Category name already exists");
        }

        var category = _mapper.Map<Category>(request);
        var createdCategory = await _categoryRepository.AddAsync(category);
        
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequestDto request)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id);
        if (existingCategory == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Check if name is being changed and if it's unique
        if (existingCategory.Name != request.Name && await _categoryRepository.IsCategoryNameExistsAsync(request.Name))
        {
            throw new InvalidOperationException("Category name already exists");
        }

        _mapper.Map(request, existingCategory);
        existingCategory.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Check if category has announces
        var categoryWithAnnounces = await _categoryRepository.GetCategoryWithAnnouncesAsync(id);
        if (categoryWithAnnounces != null && categoryWithAnnounces.Announces.Any())
        {
            throw new InvalidOperationException("Cannot delete category that has announces");
        }

        await _categoryRepository.DeleteAsync(id);
    }

    public async Task<bool> IsCategoryNameExistsAsync(string name)
    {
        return await _categoryRepository.IsCategoryNameExistsAsync(name);
    }
}