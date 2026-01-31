namespace UniversityManagementSystem.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int AvailableSeats { get; set; }
    public string? DepartmentName { get; set; }
    public string? InstructorFullName { get; set; }
    public bool IsActive { get; set; }
}