using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Notification;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;
using System.Security.Claims;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetNotifications()
    {
        try
        {
            var username = GetCurrentUsername();
            var notifications = await _notificationService.GetNotificationsByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResult(notifications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<NotificationDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("unread")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetUnreadNotifications()
    {
        try
        {
            var username = GetCurrentUsername();
            var notifications = await _notificationService.GetUnreadNotificationsByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResult(notifications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<NotificationDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadNotificationsCount()
    {
        try
        {
            var username = GetCurrentUsername();
            var count = await _notificationService.GetUnreadCountByUsernameAsync(username);
            return Ok(ApiResponse<int>.SuccessResult(count));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<int>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetNotification(Guid id)
    {
        try
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound(ApiResponse<NotificationDto>.ErrorResult("Notification not found"));
            }

            var username = GetCurrentUsername();
            if (notification.Username != username)
            {
                return Forbid("You can only view your own notifications");
            }

            return Ok(ApiResponse<NotificationDto>.SuccessResult(notification));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<NotificationDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id:guid}/mark-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            await _notificationService.MarkAsReadAsync(id, username);
            return Ok(ApiResponse<object>.SuccessResult(null, "Notification marked as read"));
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

    [HttpPut("mark-all-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        try
        {
            var username = GetCurrentUsername();
            await _notificationService.MarkAllAsReadAsync(username);
            return Ok(ApiResponse<object>.SuccessResult(null, "All notifications marked as read"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            await _notificationService.DeleteNotificationAsync(id, username);
            return Ok(ApiResponse<object>.SuccessResult(null, "Notification deleted successfully"));
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

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? 
               User.FindFirst("username")?.Value ?? 
               throw new UnauthorizedAccessException("Username not found in token");
    }
}