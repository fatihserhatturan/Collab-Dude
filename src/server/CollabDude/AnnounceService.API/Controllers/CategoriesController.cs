using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Category;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetActiveCategories()
    {
        try
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResult("Category not found"));
            }
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryByName(string name)
    {
        try
        {
            var category = await _categoryService.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResult("Category not found"));
            }
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Only admins can create categories
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(CreateCategoryRequestDto request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id },
                ApiResponse<CategoryDto>.SuccessResult(category, "Category created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, UpdateCategoryRequestDto request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<CategoryDto>.ErrorResult("ID mismatch"));
            }

            var category = await _categoryService.UpdateCategoryAsync(request);
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category, "Category updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(Guid id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Category deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("check-name/{name}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckCategoryNameExists(string name)
    {
        try
        {
            var exists = await _categoryService.IsCategoryNameExistsAsync(name);
            return Ok(ApiResponse<bool>.SuccessResult(exists));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
}