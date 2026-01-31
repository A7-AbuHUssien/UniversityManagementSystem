namespace UniversityManagementSystem.Application.DTOs.Dashboard_DTOs;

public class CriticalCourseDto
{
    public string CourseTitle { get; set; }
    public int CurrentStudents { get; set; }
    public int MaxCapacity { get; set; }
    public double FullnessPercentage { get; set; }
}