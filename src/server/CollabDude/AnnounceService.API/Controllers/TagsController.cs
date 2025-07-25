using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Tag;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTags()
    {
        try
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(ApiResponse<IEnumerable<TagDto>>.SuccessResult(tags));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TagDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetActiveTags()
    {
        try
        {
            var tags = await _tagService.GetActiveTagsAsync();
            return Ok(ApiResponse<IEnumerable<TagDto>>.SuccessResult(tags));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TagDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TagDto>>> GetTag(Guid id)
    {
        try
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound(ApiResponse<TagDto>.ErrorResult("Tag not found"));
            }
            return Ok(ApiResponse<TagDto>.SuccessResult(tag));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TagDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("announce/{announceId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTagsByAnnounce(Guid announceId)
    {
        try
        {
            var tags = await _tagService.GetTagsByAnnounceIdAsync(announceId);
            return Ok(ApiResponse<IEnumerable<TagDto>>.SuccessResult(tags));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TagDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TagDto>>> CreateTag(CreateTagRequestDto request)
    {
        try
        {
            var tag = await _tagService.CreateTagAsync(request);
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id },
                ApiResponse<TagDto>.SuccessResult(tag, "Tag created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TagDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTag(Guid id)
    {
        try
        {
            await _tagService.DeleteTagAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Tag deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}