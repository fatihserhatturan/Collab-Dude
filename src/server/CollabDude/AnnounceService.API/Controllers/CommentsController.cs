using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnnounceService.Application.DTOs.Comments;
using AnnounceService.Application.DTOs.Common;
using AnnounceService.Application.Interfaces;
using System.Security.Claims;

namespace AnnounceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> GetComment(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(ApiResponse<CommentDto>.ErrorResult("Comment not found"));
            }
            return Ok(ApiResponse<CommentDto>.SuccessResult(comment));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("announce/{announceId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetCommentsByAnnounce(Guid announceId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByAnnounceIdAsync(announceId);
            return Ok(ApiResponse<IEnumerable<CommentDto>>.SuccessResult(comments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CommentDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("my-comments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetMyComments()
    {
        try
        {
            var username = GetCurrentUsername();
            var comments = await _commentService.GetCommentsByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<CommentDto>>.SuccessResult(comments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CommentDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("user/{username}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetUserComments(string username)
    {
        try
        {
            var comments = await _commentService.GetCommentsByUsernameAsync(username);
            return Ok(ApiResponse<IEnumerable<CommentDto>>.SuccessResult(comments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CommentDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CommentDto>>> CreateComment(CreateCommentRequestDto request)
    {
        try
        {
            var username = GetCurrentUsername();
            var comment = await _commentService.CreateCommentAsync(request, username);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id },
                ApiResponse<CommentDto>.SuccessResult(comment, "Comment created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CommentDto>>> UpdateComment(Guid id, UpdateCommentRequestDto request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<CommentDto>.ErrorResult("ID mismatch"));
            }

            var username = GetCurrentUsername();
            var comment = await _commentService.UpdateCommentAsync(request, username);
            return Ok(ApiResponse<CommentDto>.SuccessResult(comment, "Comment updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteComment(Guid id)
    {
        try
        {
            var username = GetCurrentUsername();
            await _commentService.DeleteCommentAsync(id, username);
            return Ok(ApiResponse<object>.SuccessResult(null, "Comment deleted successfully"));
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