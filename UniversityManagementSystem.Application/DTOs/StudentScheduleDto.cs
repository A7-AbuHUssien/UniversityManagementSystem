namespace UniversityManagementSystem.Application.DTOs;

public class StudentScheduleDto
{
    public string CourseTitle { get; set; } = string.Empty;
    public string InstructorName { get; set; } = "TBD"; 
    public DayOfWeek Day { get; set; } 
    public int Hour { get; set; }
    
}