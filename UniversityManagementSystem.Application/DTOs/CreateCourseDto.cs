namespace UniversityManagementSystem.Application.DTOs;

public class CreateCourseDto
{
    public string Title { get; set; }
    public string CourseCode { get; set; }
    public int Credits { get; set; }
    public int DepartmentId { get; set; }
    public int InstructorId { get; set; }
    public int MaxCapacity { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int Hour { get; set; }
}