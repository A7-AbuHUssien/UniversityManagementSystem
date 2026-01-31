namespace UniversityManagementSystem.Application.DTOs.Identity_DTOs;

public class RegisterInstructorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}