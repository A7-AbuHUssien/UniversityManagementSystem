namespace UniversityManagementSystem.Application.DTOs;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }

    // Constructor للنجاح
    public ApiResponse(T data, string message = null)
    {
        Succeeded = true;
        Message = message;
        Data = data;
    }

    // Constructor للفشل
    public ApiResponse(string message)
    {
        Succeeded = false;
        Message = message;
    }
    
}