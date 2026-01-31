using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Api.Filters;

public class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is not ApiResponse<object>)
        {
            if (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
            {
                var response = new ApiResponse<object>(objectResult.Value)
                {
                    Succeeded = true,
                    Message = "Operation completed successfully."
                };
                objectResult.Value = response;
            }
        }
        await next();
    }
}