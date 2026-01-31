namespace UniversityManagementSystem.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; } = 0;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalEmail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public int DepartmentId { get; set; } 
}