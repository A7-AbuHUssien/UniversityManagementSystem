namespace UniversityManagementSystem.Application.DTOs;

public class AssignGradeDto 
{
    public int studentId { get; set; }
    public int courseId { get; set; }
    public decimal Score { get; set; }
}