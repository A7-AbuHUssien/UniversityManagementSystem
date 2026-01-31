namespace UniversityManagementSystem.Application.DTOs;

public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ErrorResponseDto(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}