namespace UniversityManagementSystem.Application.DTOs;

public class StudentScheduleDto
{
    public string CourseTitle { get; set; } = string.Empty;
    public string InstructorName { get; set; } = "TBD"; 
    public string Day { get; set; } = string.Empty;
    public int Hour { get; set; }
    
}