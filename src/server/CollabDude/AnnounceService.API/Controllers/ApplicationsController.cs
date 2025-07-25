using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Application;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;
using System.Security.Claims;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> GetApplication(Guid id)
    {
        try
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound(ApiResponse<ApplicationDto>.ErrorResult("Application not found"));
            }

            var username = GetCurrentUsername();
            
            // Only allow owner or announce owner to view application details
            if (application.ApplicantUsername != username && 
                application.Announce?.Username != username)
            {
                return Forbid("You can only view your own applications or applications to your announces");
            }

            return Ok(ApiResponse<ApplicationDto>.SuccessResult(application));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApplicationDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("announce/{announceId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationDto>>>> GetApplicationsByAnnounce(Guid announceId)
    {
        try
        {
            var applications = await _applicationService.GetApplicationsByAnnounceIdAsync(announceId);
            return Ok(ApiResponse<IEnumerable<ApplicationDto>>.SuccessResult(applications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ApplicationDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("my-applications")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationDto>>>> GetMyApplications()
    {
        try
        {
            var username = GetCurrentUsername();
            var applications = await _applicationService.GetApplicationsByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<ApplicationDto>>.SuccessResult(applications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ApplicationDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> CreateApplication(CreateApplicationRequestDto request)
    {
        try
        {
            var username = GetCurrentUsername();
            
            // Check if user can apply
            if (!await _applicationService.CanUserApplyAsync(request.AnnounceId, username))
            {
                return BadRequest(ApiResponse<ApplicationDto>.ErrorResult("You cannot apply to this announce"));
            }

            var application = await _applicationService.CreateApplicationAsync(request, username);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id },
                ApiResponse<ApplicationDto>.SuccessResult(application, "Application submitted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApplicationDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> UpdateApplicationStatus(Guid id, UpdateApplicationStatusRequestDto request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<ApplicationDto>.ErrorResult("ID mismatch"));
            }

            var username = GetCurrentUsername();
            var application = await _applicationService.UpdateApplicationStatusAsync(request, username);
            return Ok(ApiResponse<ApplicationDto>.SuccessResult(application, "Application status updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApplicationDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteApplication(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            await _applicationService.DeleteApplicationAsync(id, username);
            return Ok(ApiResponse<object>.SuccessResult(null, "Application withdrawn successfully"));
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

    [HttpGet("announce/{announceId:guid}/can-apply")]
    public async Task<ActionResult<ApiResponse<bool>>> CanApply(Guid announceId)
    {
        try
        {
            var username = GetCurrentUsername();
            var canApply = await _applicationService.CanUserApplyAsync(announceId, username);
            return Ok(ApiResponse<bool>.SuccessResult(canApply));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("announce/{announceId:guid}/has-applied")]
    public async Task<ActionResult<ApiResponse<bool>>> HasApplied(Guid announceId)
    {
        try
        {
            var username = GetCurrentUsername();
            var hasApplied = await _applicationService.HasUserAppliedAsync(announceId, username);
            return Ok(ApiResponse<bool>.SuccessResult(hasApplied));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? 
               User.FindFirst("username")?.Value ?? 
               throw new UnauthorizedAccessException("Username not found in token");
    }
}