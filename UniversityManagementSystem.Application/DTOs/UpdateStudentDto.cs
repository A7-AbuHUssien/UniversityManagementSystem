namespace UniversityManagementSystem.Application.DTOs;

public class UpdateStudentDto
{
    public int Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? DepartmentName { get; set; }
}