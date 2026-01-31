namespace UniversityManagementSystem.Application.DTOs.Identity_DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string AcademicEmail { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public List<string> Roles { get; set; } = new();
}