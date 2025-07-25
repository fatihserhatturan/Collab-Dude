using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Announce;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;
using System.Security.Claims;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncesController : ControllerBase
{
    private readonly IAnnounceService _announceService;

    public AnnouncesController(IAnnounceService announceService)
    {
        _announceService = announceService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AnnounceDto>>>> GetAnnounces([FromQuery] AnnounceSearchDto searchDto)
    {
        try
        {
            var announces = await _announceService.GetAnnouncesAsync(searchDto);
            return Ok(ApiResponse<PagedResult<AnnounceDto>>.SuccessResult(announces));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<AnnounceDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AnnounceDto>>> GetAnnounce(Guid id)
    {
        try
        {
            var announce = await _announceService.GetAnnounceByIdAsync(id);
            if (announce == null)
            {
                return NotFound(ApiResponse<AnnounceDto>.ErrorResult("Announce not found"));
            }
            return Ok(ApiResponse<AnnounceDto>.SuccessResult(announce));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AnnounceDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<ApiResponse<AnnounceDetailDto>>> GetAnnounceDetails(Guid id)
    {
        try
        {
            var announce = await _announceService.GetAnnounceDetailByIdAsync(id);
            if (announce == null)
            {
                return NotFound(ApiResponse<AnnounceDetailDto>.ErrorResult("Announce not found"));
            }
            return Ok(ApiResponse<AnnounceDetailDto>.SuccessResult(announce));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AnnounceDetailDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AnnounceDto>>>> GetAnnouncesByCategory(Guid categoryId)
    {
        try
        {
            var announces = await _announceService.GetAnnouncesByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<AnnounceDto>>.SuccessResult(announces));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AnnounceDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("my-announces")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<AnnounceDto>>>> GetMyAnnounces()
    {
        try
        {
            var username = GetCurrentUsername();
            var announces = await _announceService.GetAnnouncesByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<AnnounceDto>>.SuccessResult(announces));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AnnounceDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("user/{username}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AnnounceDto>>>> GetUserAnnounces(string username)
    {
        try
        {
            var announces = await _announceService.GetAnnouncesByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<AnnounceDto>>.SuccessResult(announces));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AnnounceDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AnnounceDto>>> CreateAnnounce(CreateAnnounceRequestDto request)
    {
        try
        {
            var username = GetCurrentUsername();
            var announce = await _announceService.CreateAnnounceAsync(request, username);
            return CreatedAtAction(nameof(GetAnnounce), new { id = announce.Id },
                ApiResponse<AnnounceDto>.SuccessResult(announce, "Announce created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AnnounceDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AnnounceDto>>> UpdateAnnounce(Guid id, UpdateAnnounceRequestDto request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<AnnounceDto>.ErrorResult("ID mismatch"));
            }

            var username = GetCurrentUsername();
            var announce = await _announceService.UpdateAnnounceAsync(request, username);
            return Ok(ApiResponse<AnnounceDto>.SuccessResult(announce, "Announce updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AnnounceDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAnnounce(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            await _announceService.DeleteAnnounceAsync(id, username);
            return Ok(ApiResponse<object>.SuccessResult(null, "Announce deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("{id:guid}/expire")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ExpireAnnounce(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            
            // Check if user owns the announce
            if (!await _announceService.IsAnnounceOwnerAsync(id, username))
            {
                return Forbid("You can only expire your own announces");
            }

            await _announceService.MarkAsExpiredAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Announce marked as expired"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? 
               User.FindFirst("username")?.Value ?? 
               throw new UnauthorizedAccessException("Username not found in token");
    }
}