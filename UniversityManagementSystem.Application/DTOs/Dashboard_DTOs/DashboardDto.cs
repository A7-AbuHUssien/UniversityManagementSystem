namespace UniversityManagementSystem.Application.DTOs.Dashboard_DTOs;

public class DashboardDto
{
    public List<CriticalCourseDto> BottleneckCourses { get; set; } = new();
    public int AtRiskStudentsCount { get; set; }
    public double AtRiskPercentage { get; set; } 
    public int StudentsWithNoEnrollments { get; set; }
    public List<HighDropRateCourseDto> ProblematicCourses { get; set; } = new();
    public List<DepartmentLoadDto> OverloadedDepartments { get; set; } = new();
}