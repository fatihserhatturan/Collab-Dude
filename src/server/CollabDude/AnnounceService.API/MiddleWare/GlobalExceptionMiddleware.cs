using System.Net;
using System.Text.Json;
using AnnounceService.Application.DTOs.Common;
using FluentValidation;

namespace AnnounceService.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null
        };

        switch (exception)
        {
            case ValidationException validationEx:
                response.Message = "Validation failed";
                response.Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case InvalidOperationException:
                response.Message = exception.Message;
                response.Errors = new List<string> { exception.Message };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ArgumentException:
                response.Message = exception.Message;
                response.Errors = new List<string> { exception.Message };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                response.Message = "Unauthorized access";
                response.Errors = new List<string> { exception.Message };
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            case KeyNotFoundException:
                response.Message = "Resource not found";
                response.Errors = new List<string> { exception.Message };
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case TimeoutException:
                response.Message = "Request timeout";
                response.Errors = new List<string> { "The request took too long to process" };
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                response.Message = "An internal server error occurred";
                response.Errors = new List<string> { "Please try again later or contact support" };
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}