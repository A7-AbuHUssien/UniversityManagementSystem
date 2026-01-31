namespace UniversityManagementSystem.Application.Entities;

public class Grade : BaseEntity
{
    public string Symbol { get; set; } 
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }
    public string Description { get; set; }
    public decimal GPAPoint { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}