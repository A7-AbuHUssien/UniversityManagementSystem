using System.Net;
using System.Text.Json;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = exception switch
        {
            InvalidOperationException or ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
        context.Response.StatusCode = statusCode;
        var response = new ApiResponse<object>(null) 
        {
            Succeeded = false,
            Message = statusCode == (int)HttpStatusCode.InternalServerError 
                ? "Internal Server Error. Please try again later." 
                : exception.Message,
            Errors = new List<string> { exception.Message } 
        };
        if (exception.InnerException != null)
        {
            response.Errors.Add(exception.InnerException.Message);
        }
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}