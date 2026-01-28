using System.Net;

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

        // Determine the status code based on the exception type
        context.Response.StatusCode = exception switch
        {
            // Business logic and validation errors
            InvalidOperationException => (int)HttpStatusCode.BadRequest, // 400
            KeyNotFoundException => (int)HttpStatusCode.NotFound,        // 404
            _ => (int)HttpStatusCode.InternalServerError                 // 500 (Generic server error)
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message, // The message provided in your Validation logic
            
            // Include stack trace only in local development for security
            Details = context.Request.Host.Host.Contains("localhost") ? exception.StackTrace : null 
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}